using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using System.Threading.Tasks;
using RPPP_WebApp.Extensions.Selectors;
using System.Linq;
using System.Text.Json;
using System;
using RPPP_WebApp.Extensions;
using Microsoft.Data.SqlClient;

namespace RPPP_WebApp.Controllers
{
    public class KriticnostController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<KriticnostController> logger;
        private readonly AppSettings appSettings;
        private readonly string title = "Kriticnosti";

        public KriticnostController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<KriticnostController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            this.appSettings = options.Value;
        }

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            int pagesize = appSettings.PageSize;
            int pageOffset = appSettings.PageOffset;
            var query = ctx.Kriticnost.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                logger.LogInformation("Ne postoji niti jedna kritičnost");
                TempData[Constants.Message] = "Ne postoji niti jedna jedna kritičnost.";
                TempData[Constants.ErrorOccurred] = false;
            }

            var pagingInfo = new PagingInfo
            {
                CurrentPage = page,
                Sort = sort,
                Ascending = ascending,
                ItemsPerPage = pagesize,
                TotalItems = count,
                PageOffset = pageOffset
            };
            if (page < 1 || page > pagingInfo.TotalPages)
            {
                return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
            }

            query = query.ApplySort(sort, ascending);

            var kriticnosti = await query
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            KriticnostViewModel kvm = new KriticnostViewModel()
            {
                Kriticnosti = kriticnosti,
                PagingInfo = pagingInfo
            };
            return View(kvm);
        }

        [HttpGet]
        public async Task<IActionResult> Dodaj()
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(Kriticnost kriticnost)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            logger.LogTrace(JsonSerializer.Serialize(kriticnost));
            if (ModelState.IsValid)
            {
                try
                {
                    await ctx.AddAsync(kriticnost);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation(new EventId(1000), $"Kritičnost uspješno dodana.");

                    TempData[Constants.Message] = $"Kritičnost uspješno dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanja kritičnosti: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(kriticnost);
                }
            }
            else
            {
                return View(kriticnost);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Uredi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            var kriticnost = await ctx.Kriticnost
                                  .AsNoTracking()
                                  .Where(k => k.Id == id)
                                  .SingleOrDefaultAsync();
            if (kriticnost != null)
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(kriticnost);
            }
            else
            {
                return NotFound($"Neispravan unos za šifru: {id}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(Kriticnost kriticnost, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            if (kriticnost == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.Kriticnost.AnyAsync(k => k.Id == kriticnost.Id);
            if (!checkId)
            {
                return NotFound($"Neispravan status: {kriticnost?.Id}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Kriticnost.Update(kriticnost);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = "Kritičnost ažurirana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());

                    return View(kriticnost);
                }
            }
            else
            {
                return View(kriticnost);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var kriticnost = await ctx.Kriticnost.FindAsync(id);
            if (kriticnost != null)
            {
                try
                {
                    ctx.Remove(kriticnost);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Kritičnost sa šifrom {id} obrisana.";
                    TempData[Constants.ErrorOccurred] = false;
                } catch (Exception exc)
                {
                    TempData[Constants.Message] = $"Pogreška prilikom brisanja kritičnosti, id: {id}: ovim stupnjem kritičnosti se koriste neki sustavi i/ili podsustavi!";
                    TempData[Constants.ErrorOccurred] = true;
                }
            }
            else
            {
                TempData[Constants.Message] = $"Ne postoji stupanj kritičnosti sa šifrom: {id}";
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
    }
}

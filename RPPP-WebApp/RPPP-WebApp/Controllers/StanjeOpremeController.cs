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

namespace RPPP_WebApp.Controllers
{

    public class StanjeOpremeController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<StanjeOpremeController> logger;
        private readonly AppSettings appSettings;
        private readonly string title = "Stanje opreme";

        public StanjeOpremeController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<StanjeOpremeController> logger)
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
            var query = ctx.Stanje.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                logger.LogInformation("Ne postoji niti jedno stanje opreme");
                TempData[Constants.Message] = "Ne postoji niti jedno stanje opreme.";
                TempData[Constants.ErrorOccurred] = false;
                return View();
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

            var stanjaOpreme = await query
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            StanjeOpremeViewModel sovm = new StanjeOpremeViewModel()
            {
                Stanja = stanjaOpreme,
                PagingInfo = pagingInfo
            };
            return View(sovm);
        }
        [HttpGet]
        public IActionResult Dodaj()
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(Stanje stanjeOpreme)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            logger.LogTrace(JsonSerializer.Serialize(stanjeOpreme));
            if (ModelState.IsValid)
            {
                try
                {
                    await ctx.AddAsync(stanjeOpreme);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation(new EventId(1000), $"Stanje opreme uspješno dodano.");

                    TempData[Constants.Message] = $"Stanje opreme uspješno dodano.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanja stanja opreme: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(stanjeOpreme);
                }
            }
            else
            {
                return View(stanjeOpreme);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Uredi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            var stanjeOpreme = await ctx.Stanje
                                  .AsNoTracking()
                                  .Where(s => s.Id == id)
                                  .SingleOrDefaultAsync();
            if (stanjeOpreme != null)
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(stanjeOpreme);
            }
            else
            {
                return NotFound($"Neispravan unos za id stanja opreme: {id}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(Stanje stanjeOpreme, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            if (stanjeOpreme == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.Stanje.AnyAsync(s => s.Id == stanjeOpreme.Id);
            if (!checkId)
            {
                return NotFound($"Neispravan status: {stanjeOpreme?.Id}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Stanje.Update(stanjeOpreme);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = "Stanje opreme ažurirano.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation($"Stanje opreme uspješno ažurirano. Id={stanjeOpreme.Id}");
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    logger.LogError("Pogreška prilikom uređivanja stanja opreme: {0}", exc.CompleteExceptionMessage());
                    return View(stanjeOpreme);
                }
            }
            else
            {
                return View(stanjeOpreme);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var stanjeOpreme = await ctx.Stanje.FindAsync(id);
            if (stanjeOpreme != null)
            {
                try
                {
                    ctx.Remove(stanjeOpreme);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Stanje opreme s id {id} obrisano.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation($"Stanje opreme uspješno obrisano. Id={id}");
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja stanja opreme: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja stanja opreme: {0}", exc.CompleteExceptionMessage());
                }
            }
            else
            {
                TempData[Constants.Message] = $"Ne postoji stanje opreme s id: {id}";
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using RPPP_WebApp.Extensions.Selectors;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;
using System;
using RPPP_WebApp.Extensions;

namespace RPPP_WebApp.Controllers
{
    public class TipOpremeController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<TipOpremeController> logger;
        private readonly AppSettings appSettings;
        private readonly string title = "Tip opreme";

        public TipOpremeController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<TipOpremeController> logger)
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

            var query = ctx.TipOpreme.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                logger.LogInformation("Ne postoji niti jedan tip opreme");
                TempData[Constants.Message] = "Ne postoji niti jedan tip opreme.";
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

            var tipovi = await query
                            .Skip((page - 1) * pagesize)
                            .Take(pagesize)
                            .ToListAsync();
            TipOpremeViewModel tovm = new TipOpremeViewModel()
            {
                Tipovi = tipovi,
                PagingInfo = pagingInfo
            };
            return View(tovm);
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
        public async Task<IActionResult> Dodaj(TipOpreme tipOpreme)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            logger.LogTrace(JsonSerializer.Serialize(tipOpreme));
            if (ModelState.IsValid)
            {
                try
                {
                    await ctx.AddAsync(tipOpreme);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation(new EventId(1000), $"Tip opreme uspješno dodan.");

                    TempData[Constants.Message] = $"Tip opreme uspješno dodan.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanja tipa opreme: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(tipOpreme);
                }
            }
            else
            {
                return View(tipOpreme);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Uredi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            var tipOpreme = await ctx.TipOpreme
                                .AsNoTracking()
                                .Where(t => t.Id == id)
                                .SingleOrDefaultAsync();
            if (tipOpreme != null)
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(tipOpreme);
            }
            else
            {
                return NotFound($"Neispravan id za tip opreme: {id}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(TipOpreme tipOpreme, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            if(tipOpreme == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.TipOpreme.AnyAsync(t => t.Id == tipOpreme.Id);
            if (!checkId)
            {
                return NotFound($"Neispravan id tipa opreme: {tipOpreme.Id}");
            }
            if(ModelState.IsValid)
            {
                try
                {
                    ctx.Update(tipOpreme);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = "Tip opreme ažuriran.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(tipOpreme);
                }
            }
            else
            {
                return View(tipOpreme);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var tipOpreme = await ctx.TipOpreme.FindAsync(id);
            if(tipOpreme != null)
            {
                try
                {
                    ctx.Remove(tipOpreme);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = "Tip opreme uspješno obrisan.";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja tipa opreme: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                }
            }
            else
            {
                TempData[Constants.Message] = $"Ne postoji tip opreme s id: {id}";
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
    }
}
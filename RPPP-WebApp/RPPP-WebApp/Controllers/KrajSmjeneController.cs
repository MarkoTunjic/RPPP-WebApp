using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Extensions;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;

namespace RPPP_WebApp.Controllers
{
    public class KrajSmjeneController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<KrajSmjeneController> logger;
        private readonly AppSettings appSettings;
        private readonly string title = "Kraj smjene";

        public KrajSmjeneController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<KrajSmjeneController> logger)
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
            var query = ctx.KrajSmjene.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                logger.LogInformation("Ne postoji niti jedan kraj smjene");
                TempData[Constants.Message] = "Ne postoji niti jedan kraj smjene.";
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

            var krajeviSmjena = await query
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            KrajSmjeneViewModel ksvm = new KrajSmjeneViewModel()
            {
                KrajeviSmjena = krajeviSmjena,
                PagingInfo = pagingInfo
            };
            return View(ksvm);
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
        public async Task<IActionResult> Dodaj(KrajSmjene krajSmjene)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            logger.LogTrace(JsonSerializer.Serialize(krajSmjene));
            if (ModelState.IsValid)
            {
                try
                {
                    await ctx.AddAsync(krajSmjene);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation(new EventId(1000), $"Kraj smjene uspjesno dodan.");

                    TempData[Constants.Message] = $"Kraj smjene uspjesno dodan.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanja kraja smjene: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(krajSmjene);
                }
            }
            else
            {
                return View(krajSmjene);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Uredi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            var krajSmjene = await ctx.KrajSmjene
                                  .AsNoTracking()
                                  .Where(k => k.Id == id)
                                  .SingleOrDefaultAsync();
            if (krajSmjene != null)
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(krajSmjene);
            }
            else
            {
                return NotFound($"Neispravno vrijeme kraja smjene sa šifrom: {id}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(KrajSmjene krajSmjene, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            if (krajSmjene == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.KrajSmjene.AnyAsync(k => k.Id == krajSmjene.Id);
            if (!checkId)
            {
                return NotFound($"Neispravno vrijeme kraja smjene sa šifrom: {krajSmjene?.Id}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(krajSmjene);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = "Kraj smjene ažuriran.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Kraj smjene ažuriran.");
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    logger.LogError("Pogreška prilikom ažuriranja kraja smjene.");
                    return View(krajSmjene);
                }
            }
            else
            {
                return View(krajSmjene);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var krajSmjene = await ctx.KrajSmjene.FindAsync(id);
            if (krajSmjene != null)
            {
                try
                {
                    ctx.Remove(krajSmjene);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Kraj smjene sa šifrom {id} obrisan.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Kraj smjene sa šifrom {id} obrisan.", id);
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja kraja smjene: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja kraja smjene: {0}", exc.CompleteExceptionMessage());
                }
            }
            else
            {
                TempData[Constants.Message] = $"Ne postoji kraj smjene sa šifrom: {id}";
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
    }
}

using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Extensions;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;

namespace RPPP_WebApp.Controllers
{
    public class RangController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<RangController> logger;
        private readonly AppSettings appSettings;
        private readonly string title = "Rang kontrolora";

        public RangController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<RangController> logger)
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
            var query = ctx.Rang.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                logger.LogInformation("Ne postoji niti jedan rang kontrolora");
                TempData[Constants.Message] = "Ne postoji niti jedan rang kontrolora.";
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

            var rangovi = await query
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            RangViewModel rvm = new RangViewModel()
            {
                Rangovi = rangovi,
                PagingInfo = pagingInfo
            };
            return View(rvm);
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
        public async Task<IActionResult> Dodaj(Rang rang)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            logger.LogTrace(JsonSerializer.Serialize(rang));
            if (ModelState.IsValid)
            {
                try
                {
                    await ctx.AddAsync(rang);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation(new EventId(1000), $"Rang uspjesno dodan.");

                    TempData[Constants.Message] = $"Rang uspjesno dodan.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanja ranga: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(rang);
                }
            }
            else
            {
                return View(rang);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Uredi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            var rang = await ctx.Rang
                                  .AsNoTracking()
                                  .Where(r => r.Id == id)
                                  .SingleOrDefaultAsync();
            if (rang != null)
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(rang);
            }
            else
            {
                return NotFound($"Neispravno ime ranga sa šifrom: {id}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(Rang rang, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            if (rang == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.Rang.AnyAsync(r => r.Id == rang.Id);
            if (!checkId)
            {
                return NotFound($"Neispravno ime ranga sa šifrom: {rang?.Id}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(rang);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = "Ime ranga ažurirano.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Ime ranga ažurirano.");
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    logger.LogError("Pogreška prilikom ažuriranja ranga");
                    return View(rang);
                }
            }
            else
            {
                return View(rang);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var rang = await ctx.Rang.FindAsync(id);
            if (rang != null)
            {
                try
                {
                    ctx.Remove(rang);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Rang sa šifrom {id} obrisan.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Rang sa šifrom {id} obrisan.", id);
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja ranga: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja ranga: {0}", exc.CompleteExceptionMessage());
                }
            }
            else
            {
                TempData[Constants.Message] = $"Ne postoji ime ranga sa šifrom: {id}";
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
    }
}

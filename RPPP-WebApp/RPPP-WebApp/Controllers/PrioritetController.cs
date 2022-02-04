using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Extensions;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RPPP_WebApp.Controllers
{
    public class PrioritetController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<PrioritetController> logger;
        private readonly AppSettings appSettings;
        private readonly string title = "Prioriteti radnog naloga";

        public PrioritetController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<PrioritetController> logger)
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
            var query = ctx.Prioritet.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                logger.LogInformation("Ne postoji niti jedan prioritet");
                TempData[Constants.Message] = "Ne postoji niti jedan prioritet.";
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

            var prioriteti = await query
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            PrioritetViewModel pvm = new PrioritetViewModel()
            {
                Prioriteti = prioriteti,
                PagingInfo = pagingInfo
            };
            return View(pvm);
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
        public async Task<IActionResult> Dodaj(Prioritet prioritet)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            logger.LogTrace(JsonSerializer.Serialize(prioritet));
            if (ModelState.IsValid)
            {
                try
                {
                    await ctx.AddAsync(prioritet);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation(new EventId(1000), $"Prioritet uspješno dodan.");

                    TempData[Constants.Message] = $"Prioritet uspješno dodan.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanja prioriteta: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(prioritet);
                }
            }
            else
            {
                return View(prioritet);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Uredi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            var prioritet = await ctx.Prioritet
                                  .AsNoTracking()
                                  .Where(p => p.Id == id)
                                  .SingleOrDefaultAsync();
            if (prioritet != null)
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(prioritet);
            }
            else
            {
                return NotFound($"Neispravan unos za prioritet: {id}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(Prioritet prioritet, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            if (prioritet == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.Prioritet.AnyAsync(p => p.Id == prioritet.Id);
            if (!checkId)
            {
                return NotFound($"Neispravan prioritet: {prioritet?.Id}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Prioritet.Update(prioritet);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = "Prioritet ažuriran.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Prioritet ažuriran.");
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    logger.LogError("Pogreška prilikom ažuriranja prioriteta.");
                    return View(prioritet);
                }
            }
            else
            {
                return View(prioritet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var prioritet = await ctx.Prioritet.FindAsync(id);
            if (prioritet != null)
            {
                try
                {
                    ctx.Remove(prioritet);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Prioritet sa šifrom {id} obrisan.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation($"Prioritet sa šifrom {id} obrisan.");
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja prioriteta: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja prioriteta: {0}", exc.CompleteExceptionMessage());
                }
            }
            else
            {
                TempData[Constants.Message] = $"Ne postoji prioritet sa šifrom: {id}";
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }

    }
}

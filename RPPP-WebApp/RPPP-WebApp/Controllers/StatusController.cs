using System;
using Microsoft.AspNetCore.Mvc;

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
    public class StatusController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<StatusController> logger;
        private readonly AppSettings appSettings;
        private readonly string title = "Statusi radnog naloga";

        public StatusController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<StatusController> logger)
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
            var query = ctx.Status.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                logger.LogInformation("Ne postoji niti jedan status");
                TempData[Constants.Message] = "Ne postoji niti jedan status.";
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

            var statusi = await query
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            StatusViewModel svm = new StatusViewModel()
            {
                Statusi = statusi,
                PagingInfo = pagingInfo
            };
            return View(svm);
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
        public async Task<IActionResult> Dodaj(Status status)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            logger.LogTrace(JsonSerializer.Serialize(status));
            if (ModelState.IsValid)
            {
                try
                {
                    await ctx.AddAsync(status);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation(new EventId(1000), $"Status uspješno dodan.");

                    TempData[Constants.Message] = $"Status uspješno dodan.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanja statusa: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(status);
                }
            }
            else
            {
                return View(status);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Uredi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            var status = await ctx.Status
                                  .AsNoTracking()
                                  .Where(s => s.Id == id)
                                  .SingleOrDefaultAsync();
            if (status != null)
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(status);
            }
            else
            {
                return NotFound($"Neispravan unos za šifru: {id}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(Status status, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            if (status == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.Status.AnyAsync(s => s.Id == status.Id);
            if (!checkId)
            {
                return NotFound($"Neispravan status: {status?.Id}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Status.Update(status);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = "Status ažuriran.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Status ažuriran.");
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    logger.LogError("Pogreška prilikom ažuriranja statusa.");
                    return View(status);
                }
            }
            else
            {
                return View(status);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var status = await ctx.Status.FindAsync(id);
            if (status != null)
            {
                try
                {
                    ctx.Remove(status);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Status sa šifrom {id} obrisan.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation($"Status sa šifrom {id} obrisan.");
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja statusa: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja statusa: {0}", exc.CompleteExceptionMessage());
                }
            }
            else
            {
                TempData[Constants.Message] = $"Ne postoji status sa šifrom: {id}";
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
    }
}

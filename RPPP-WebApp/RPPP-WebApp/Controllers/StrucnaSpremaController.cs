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

    public class StrucnaSpremaController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<StrucnaSpremaController> logger;
        private readonly AppSettings appSettings;
        private readonly string title = "Strucna sprema";

        public StrucnaSpremaController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<StrucnaSpremaController> logger)
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
            var query = ctx.StrucnaSprema.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                TempData[Constants.Message] = "Ne postoji niti jedna stručna sprema.";
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

            var strucneSpreme = await query
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            StrucnaSpremaViewModel srvm = new StrucnaSpremaViewModel()
            {
                StrucneSpreme = strucneSpreme,
                PagingInfo = pagingInfo
            };
            return View(srvm);
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
        public async Task<IActionResult> Dodaj(StrucnaSprema strucnaSprema)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            logger.LogTrace(JsonSerializer.Serialize(strucnaSprema));
            if (ModelState.IsValid)
            {
                try
                {
                    await ctx.AddAsync(strucnaSprema);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation($"Stručna sprema uspješno dodana. Id={strucnaSprema.Id}");

                    TempData[Constants.Message] = $"Stručna sprema uspješno dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError(exc, "Pogreška prilikom dodavanja stručne spreme: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(strucnaSprema);
                }
            }
            else
            {
                return View(strucnaSprema);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Uredi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            var strucnaSprema = await ctx.StrucnaSprema
                                  .AsNoTracking()
                                  .Where(s => s.Id == id)
                                  .SingleOrDefaultAsync();
            if (strucnaSprema != null)
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(strucnaSprema);
            }
            else
            {
                return NotFound($"Neispravan unos za šifru: {id}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(StrucnaSprema strucnaSprema, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            if (strucnaSprema == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.Status.AnyAsync(s => s.Id == strucnaSprema.Id);
            if (!checkId)
            {
                return NotFound($"Neispravan status: {strucnaSprema?.Id}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.StrucnaSprema.Update(strucnaSprema);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = "Stručna sprema ažurirana.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation($"Stručna sprema uspješno ažurirana. Id={strucnaSprema.Id}");
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    logger.LogError(exc, "Pogreška prilikom uređivanja stručne spreme: {0}", exc.CompleteExceptionMessage());
                    return View(strucnaSprema);
                }
            }
            else
            {
                return View(strucnaSprema);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var status = await ctx.StrucnaSprema.FindAsync(id);
            if (status != null)
            {
                try
                {
                    ctx.Remove(status);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Stručna sprema sa šifrom {id} obrisana.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation($"Stručna sprema uspješno obrisana. Id={id}");
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja stručne spreme: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError(exc, "Pogreška prilikom brisanja stručne spreme: {0}", exc.CompleteExceptionMessage());
                }
            }
            else
            {
                TempData[Constants.Message] = $"Ne postoji stručna sprema sa šifrom: {id}";
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
    }
}

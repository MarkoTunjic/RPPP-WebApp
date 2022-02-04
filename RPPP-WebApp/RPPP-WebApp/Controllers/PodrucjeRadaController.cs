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
    public class PodrucjeRadaController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<PodrucjeRadaController> logger;
        private readonly AppSettings appSettings;
        private readonly string title = "Područje rada";

        public PodrucjeRadaController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<PodrucjeRadaController> logger)
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
            var query = ctx.PodrucjeRada.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                TempData[Constants.Message] = "Ne postoji niti jedno podrucje rada.";
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

            var podrucja = await query
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            PodrucjeRadaViewModel prvm = new PodrucjeRadaViewModel()
            {
                Podrucja = podrucja,
                PagingInfo = pagingInfo
            };
            return View(prvm);
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
        public async Task<IActionResult> Dodaj(PodrucjeRada podrucje)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            logger.LogTrace(JsonSerializer.Serialize(podrucje));
            if (ModelState.IsValid)
            {
                try
                {
                    await ctx.AddAsync(podrucje);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation($"Podrucje rada uspješno dodano. Id={podrucje.Id}");

                    TempData[Constants.Message] = $"Podrucje rada uspješno dodano.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError(exc, "Pogreška prilikom dodavanja podrucja rada: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(podrucje);
                }
            }
            else
            {
                return View(podrucje);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Uredi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            var podrucje = await ctx.PodrucjeRada
                                  .AsNoTracking()
                                  .Where(p => p.Id == id)
                                  .SingleOrDefaultAsync();
            if (podrucje != null)
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(podrucje);
            }
            else
            {
                return NotFound($"Neispravan unos za šifru: {id}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(PodrucjeRada podrucje, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            if (podrucje == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.Status.AnyAsync(p => p.Id == podrucje.Id);
            if (!checkId)
            {
                return NotFound($"Neispravan status: {podrucje?.Id}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(podrucje);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = "Područje ažurirano.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation($"Podrucje rada uspješno ažurirano. Id={podrucje.Id}");
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    logger.LogError(exc, "Pogreška prilikom uredivanja podrucja rada: {0}", exc.CompleteExceptionMessage());
                    return View(podrucje);
                }
            }
            else
            {
                return View(podrucje);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var podrucje = await ctx.PodrucjeRada.FindAsync(id);
            if (podrucje != null)
            {
                try
                {
                    ctx.Remove(podrucje);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Područje rada sa šifrom {id} obrisan.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation($"Podrucje rada uspješno obrisano. Id={podrucje.Id}");
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja područja rada: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError(exc, "Pogreška prilikom brisanja podrucja rada: {0}", exc.CompleteExceptionMessage());
                }
            }
            else
            {
                TempData[Constants.Message] = $"Ne postoji područje rada sa šifrom: {id}";
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
    }
}

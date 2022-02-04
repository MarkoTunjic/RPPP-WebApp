using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Extensions;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using Smjena = RPPP_WebApp.Models.Smjena;

namespace RPPP_WebApp.Controllers
{
    public class KontrolorController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<KontrolorController> logger;
        private readonly AppSettings appSettings;
        private readonly string title = "Kontrolor";

        public KontrolorController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<KontrolorController> logger)
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
            var query = ctx.Kontrolor.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                logger.LogInformation("Ne postoji niti jedan kontrolor.");
                TempData[Constants.Message] = "Ne postoji niti jedan kontrolor.";
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

            var kontrolori = await query
                          .Select(k => new KontrolorViewModel
                          {
                              Id = k.Id,
                              Ime = k.Ime,
                              Prezime = k.Prezime,
                              Oib = k.Oib,
                              DatumZaposlenja = k.DatumZaposlenja,
                              ZaposlenDo = k.ZaposlenDo,
                              KorisnickoIme = k.KorisnickoIme,
                              Lozinka = k.Lozinka,
                              ImeRanga = k.IdRangNavigation.ImeRanga,
                              PocetakSmjene = k.IdSmjenaNavigation.PocetakSmjene
                          })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            var model = new KontroloriViewModel
            {
                Kontrolori = kontrolori,
                PagingInfo = pagingInfo
            };
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Dodaj()
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            await PrepareDropDownLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(Kontrolor kontrolor)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            logger.LogTrace(JsonSerializer.Serialize(kontrolor));
            if (ModelState.IsValid)
            {
                try
                {
                    await ctx.AddAsync(kontrolor);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation(new EventId(1000), $"Kontrolor uspješno dodan.");

                    TempData[Constants.Message] = $"Kontrolor uspješno dodan.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanja novog kontrolora: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownLists();
                    return View(kontrolor);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return View(kontrolor);
            }
        }

        private async Task PrepareDropDownLists()
        {
            var smjene = await ctx.Smjena
                                  .OrderBy(s => s.PocetakSmjene)
                                  .Select(s => new { s.Id, s.PocetakSmjene })
                                  .ToListAsync();
            ViewBag.Smjene = new SelectList(smjene, nameof(Smjena.Id), nameof(Smjena.PocetakSmjene));

            var rangovi = await ctx.Rang
                                  .OrderBy(r => r.ImeRanga)
                                  .Select(r => new { r.Id, r.ImeRanga })
                                  .ToListAsync();
            ViewBag.Rangovi = new SelectList(rangovi, nameof(Rang.Id), nameof(Rang.ImeRanga));

        }

        [HttpGet]
        public async Task<IActionResult> Uredi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            var kontrolor = await ctx.Kontrolor
                                  .AsNoTracking()
                                  .Where(k => k.Id == id)
                                  .SingleOrDefaultAsync();

            if (kontrolor != null)
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                await PrepareDropDownLists();
                return View(kontrolor);
            }
            else
            {
                return NotFound($"Neispravan id kontrolora: {id}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(Kontrolor kontrolor, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            if (kontrolor == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.Kontrolor.AnyAsync(k => k.Id == kontrolor.Id);
            if (!checkId)
            {
                return NotFound($"Neispravan id kontrolora: {kontrolor?.Id}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(kontrolor);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = "Kontrolor ažuriran.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Kontrolor ažuriran.");
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownLists();
                    logger.LogError("Pogreška prilikom ažuriranja kontrolora");
                    return View(kontrolor);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return View(kontrolor);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var kontrolor = await ctx.Kontrolor.FindAsync(id);
            if (kontrolor != null)
            {
                try
                {
                    ctx.Remove(kontrolor);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Kontrolor sa šifrom {id} obrisan.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Kontrolor sa šifrom {id} obrisan.", id);
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja kontrolora: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja kontrolora: {0}", exc.CompleteExceptionMessage());
                }
            }
            else
            {
                TempData[Constants.Message] = $"Ne postoji kontrolor sa šifrom: {id}";
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
    }
}
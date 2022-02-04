using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class RadniListController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<RadniListController> logger;
        private readonly AppSettings appSettings;
        private readonly string title = "Radni list";

        public RadniListController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<RadniListController> logger)
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
            var query = ctx.RadniList.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                logger.LogInformation("Ne postoji niti jedan radni list");
                TempData[Constants.Message] = "Ne postoji niti jedan radni list.";
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

            var radniListovi = await query
                          .Select(r => new RadniListViewModel
                          {
                              Id = r.Id,
                              PocetakRada = r.PocetakRada.Date,
                              TrajanjeRada = r.TrajanjeRada,
                              OpisRada = r.OpisRada,
                              NazivUredaja = r.IdUredajNavigation.Naziv,
                              TimZaOdrzavanje = r.IdTimZaOdrzavanjeNavigation.NazivTima,
                              Status = r.IdStatusNavigation.NazivStatusa
                          })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            var model = new RadniListoviViewModel
            {
                RadniListovi = radniListovi,
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
        public async Task<IActionResult> Dodaj(RadniList radniList)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            logger.LogTrace(JsonSerializer.Serialize(radniList));
            if (ModelState.IsValid)
            {
                try
                {
                    await ctx.AddAsync(radniList);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation(new EventId(1000), $"Radni list uspješno dodan.");

                    TempData[Constants.Message] = $"Radni list uspješno dodan.";
                    TempData[Constants.ErrorOccurred] = false;
                    await PrepareDropDownLists();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanja novog radnog lista: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownLists();
                    return View(radniList);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return View(radniList);
            }
        }

        private async Task PrepareDropDownLists()
        {
            var radniNalozi = await ctx.RadniNalog
                                  .OrderBy(r => r.PocetakRada)
                                  .Select(r => new { r.Id, r.TipRada })
                                  .ToListAsync();
            ViewBag.RadniNalozi = new SelectList(radniNalozi, nameof(RadniNalog.Id), nameof(RadniNalog.TipRada));

            var timovi = await ctx.TimZaOdrzavanje
                                  .OrderBy(t => t.NazivTima)
                                  .Select(t => new { t.Id, t.NazivTima })
                                  .ToListAsync();
            ViewBag.Timovi = new SelectList(timovi, nameof(TimZaOdrzavanje.Id), nameof(TimZaOdrzavanje.NazivTima));

            var uredaji = await ctx.Uredaj
                                  .OrderBy(u => u.Naziv)
                                  .Select(u => new { u.Id, u.Naziv })
                                  .ToListAsync();
            ViewBag.Uredaji = new SelectList(uredaji, nameof(Uredaj.Id), nameof(Uredaj.Naziv));

            var statusi = await ctx.Status
                                  .OrderBy(s => s.NazivStatusa)
                                  .Select(s => new { s.Id, s.NazivStatusa })
                                  .ToListAsync();
            ViewBag.Statusi = new SelectList(statusi, nameof(Status.Id), nameof(Status.NazivStatusa));
        }

        [HttpGet]
        public async Task<IActionResult> Uredi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            var radniList = await ctx.RadniList
                                  .AsNoTracking()
                                  .Where(r => r.Id == id)
                                  .SingleOrDefaultAsync();

            if (radniList != null)
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                await PrepareDropDownLists();
                return View(radniList);
            }
            else
            {
                return NotFound($"Neispravan id radnog lista: {id}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(RadniList radniList, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            if (radniList == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.RadniList.AnyAsync(r => r.Id == radniList.Id);
            if (!checkId)
            {
                return NotFound($"Neispravan id radnog lista: {radniList?.Id}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(radniList);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = "Radni list ažuriran.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Radni list ažuriran.");
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownLists();
                    logger.LogError("Pogreška prilikom ažuriranja radnog lista.");
                    return View(radniList);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return View(radniList);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var radniList = await ctx.RadniList.FindAsync(id);
            if (radniList != null)
            {
                try
                {
                    ctx.Remove(radniList);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Radni list sa šifrom {id} obrisan.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation($"Radni list sa šifrom {id} obrisan.");
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja radnog lista: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja radnog lista: {0}", exc.CompleteExceptionMessage());
                }
            }
            else
            {
                TempData[Constants.Message] = $"Ne postoji radni list sa šifrom: {id}";
                TempData[Constants.ErrorOccurred] = true;

            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
    }
}

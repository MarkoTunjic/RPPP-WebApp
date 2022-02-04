using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;
using System.Collections.Generic;
using System.Linq;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.ViewModels;
using System.Threading.Tasks;
using System.Text.Json;
using System;
using RPPP_WebApp.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RPPP_WebApp.Controllers
{

    public class PlanOdrzavanjaController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<PlanOdrzavanjaController> logger;
        private readonly AppSettings appSettings;
        private readonly string title = "Plan održavanja";

        public PlanOdrzavanjaController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<PlanOdrzavanjaController> logger)
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
            var query = ctx.PlanOdrzavanja.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                TempData[Constants.Message] = "Ne postoji niti jedan plan održavanja.";
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

            var planoviOdrzavanja = await query
                          .Select(m => new PlanOdrzavanjaViewModel
                          {
                              DatumOdrzavanja = m.DatumOdrzavanja,
                              NazivPodsustava = m.IdPodsustavNavigation.Naziv,
                              NazivTima = m.IdTimZaOdrzavanjeNavigation.NazivTima,
                              RazinaStrucnosti = m.RazinaStrucnosti,
                              Id = m.Id
                          })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            var model = new PlanoviOdrzavanjaViewModel
            {
                PlanoviOdrzavanja = planoviOdrzavanja,
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
        public async Task<IActionResult> Dodaj(PlanOdrzavanja planOdrzavanja)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            logger.LogTrace(JsonSerializer.Serialize(planOdrzavanja));
            if (ModelState.IsValid)
            {
                try
                {
                    await ctx.AddAsync(planOdrzavanja);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation($"Plan održavanja uspješno dodan. Id={planOdrzavanja.Id}");

                    TempData[Constants.Message] = $"Plan održavanja uspješno dodan.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError(exc, "Pogreška prilikom dodavanja novog plana održavanja: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownLists();
                    return View(planOdrzavanja);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return View(planOdrzavanja);
            }
        }

        private async Task PrepareDropDownLists()
        {
            var podsustavi = await ctx.Podsustav
                                  .OrderBy(p => p.Naziv)
                                  .Select(p => new { p.Id, p.Naziv })
                                  .ToListAsync();
            ViewBag.Podsustavi = new SelectList(podsustavi, nameof(Podsustav.Id), nameof(Podsustav.Naziv));

            var timovi = await ctx.TimZaOdrzavanje
                                  .OrderBy(t => t.NazivTima)
                                  .Select(t => new { t.Id, t.NazivTima })
                                  .ToListAsync();
            ViewBag.Timovi = new SelectList(timovi, nameof(TimZaOdrzavanje.Id), nameof(TimZaOdrzavanje.NazivTima));
        }

        [HttpGet]
        public async Task<IActionResult> Uredi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            var planOdrzavanja = await ctx.PlanOdrzavanja
                                  .AsNoTracking()
                                  .Where(p => p.Id == id)
                                  .SingleOrDefaultAsync();

            if (planOdrzavanja != null)
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                await PrepareDropDownLists();
                return View(planOdrzavanja);
            }
            else
            {
                return NotFound($"Neispravan id plana održavanja: {id}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(PlanOdrzavanja planOdrzavanja, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            if (planOdrzavanja == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.PlanOdrzavanja.AnyAsync(p => p.Id == planOdrzavanja.Id);
            if (!checkId)
            {
                return NotFound($"Neispravan id plana održavanja: {planOdrzavanja?.Id}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(planOdrzavanja);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = "Plan održavanja ažuriran.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation($"Plan održavanja uspješno uređen. Id={planOdrzavanja.Id}");
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownLists();
                    logger.LogError(exc, "Pogreška prilikom uredivanja plana održavanja: {0}", exc.CompleteExceptionMessage());
                    return View(planOdrzavanja);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return View(planOdrzavanja);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var planOdrzavanja = await ctx.PlanOdrzavanja.FindAsync(id);
            if (planOdrzavanja != null)
            {
                try
                {
                    ctx.Remove(planOdrzavanja);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Plan održavanja sa šifrom {id} obrisan.";
                    logger.LogInformation($"Plan održavanja uspješno obrisan. Id={planOdrzavanja.Id}");
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja plana održavanja: " + exc.CompleteExceptionMessage();
                    logger.LogError(exc, "Pogreška prilikom brisanja plana održavanja: {0}", exc.CompleteExceptionMessage());
                    TempData[Constants.ErrorOccurred] = true;
                }
            }
            else
            {
                TempData[Constants.Message] = $"Ne postoji plan održavanja sa šifrom: {id}";
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
    }
}

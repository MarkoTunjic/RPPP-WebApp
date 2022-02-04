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

    public class FunkcijeController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<FunkcijeController> logger;
        private readonly AppSettings appSettings;
        private readonly string title = "Funkcija";

        public FunkcijeController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<FunkcijeController> logger)
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
            var query = ctx.Funkcije.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                string message = "Ne postoji niti jedna funkcija";
                logger.LogInformation(message);
                TempData[Constants.Message] = message;
                TempData[Constants.ErrorOccurred] = false;
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

            var funkcije = await query
                           .Select(f => new FunkcijaViewModel
                           {
                               Naziv = f.Naziv,
                               NazivPodsustav = f.IdPodsustavNavigation.Naziv,
                               Kategorija = f.Kategorija,
                               Id = f.Id,
                               IdPodsustav = f.IdPodsustav,
                           })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();


            var model = new FunkcijeViewModel
            {
                Funkcije = funkcije,
                PagingInfo = pagingInfo
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Dodaj()
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            await PrepareDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(Funkcije funkcija)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            logger.LogTrace(JsonSerializer.Serialize(funkcija));
            if (ModelState.IsValid)
            {
                try
                {
                    await ctx.AddAsync(funkcija);
                    await ctx.SaveChangesAsync();
                    string message = "Funkcije uspješno dodana.";
                    logger.LogInformation(new EventId(1000), message);

                    TempData[Constants.Message] = message;
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanja nove funkcije: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(funkcija);
                }
            }
            else
            {
                return View(funkcija);
            }
        }

        private async Task PrepareDropDownList()
        {
            var podsustavi = await ctx.Podsustav
                                  .OrderBy(p => p.Naziv)
                                  .Select(p => new { p.Id, p.Naziv })
                                  .ToListAsync();
            ViewBag.Podsustavi = new SelectList(podsustavi, nameof(Podsustav.Id), nameof(Podsustav.Naziv));
        }

        [HttpGet]
        public async Task<IActionResult> Uredi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            var funkcije = await ctx.Funkcije
                                  .AsNoTracking()
                                  .Where(f => f.Id == id)
                                  .SingleOrDefaultAsync();

            if (funkcije != null)
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                await PrepareDropDownList();
                return View(funkcije);
            }
            else
            {
                return NotFound($"Neispravan id funkcije: {id}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(Funkcije funkcija, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            if (funkcija == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.Funkcije.AnyAsync(f => f.Id == funkcija.Id);
            if (!checkId)
            {
                return NotFound($"Neispravan id: {funkcija?.Id}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(funkcija);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = "Funkcija ažurirana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownList();
                    return View(funkcija);
                }
            }
            else
            {
                await PrepareDropDownList();
                return View(funkcija);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id, int idPodsustav, int page = 1, int sort = 1, bool ascending = true)
        {
            var funkcije = await ctx.Funkcije.FindAsync(id, idPodsustav);
            if (funkcije != null)
            {
                try
                {
                    ctx.Remove(funkcije);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Funkcija sa šifrom {id} obrisan.";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja funkcije: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                }
            }
            else
            {
                TempData[Constants.Message] = $"Ne postoji funkcija sa šifrom: {id}, {idPodsustav}";
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
    }
}

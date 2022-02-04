using System;
using RPPP_WebApp.Extensions;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RPPP_WebApp.Controllers
{
    public class OpremaController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<OpremaController> logger;
        private readonly AppSettings appSettings;
        private readonly string title = "Oprema";

        public OpremaController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<OpremaController> logger)
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
            var query = ctx.Oprema.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                logger.LogInformation("Ne postoji niti jedna oprema");
                TempData[Constants.Message] = "Ne postoji niti jedna oprema.";
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

            var opremaList = await query
                                .Select(t => new OpremaViewModel
                                {
                                    Id = t.Id,
                                    Redundantnost = t.Redundantnost,
                                    Budzet = t.Budzet,
                                    DatumPustanjaUPogon = t.DatumPustanjaUPogon,
                                    TipOpreme = t.IdTipOpremeNavigation.TipOpreme1,
                                    PodsustavNaziv = t.IdPodsustavNavigation.Naziv,
                                    Uredaji = t.Uredaj
                                                .Select(u => new UredajViewModel
                                                {
                                                    Id = u.Id,
                                                    Naziv = u.Naziv,
                                                    Proizvodac = u.Proizvodac,
                                                    GodinaProizvodnje = u.GodinaProizvodnje,
                                                    IdOprema = u.IdOprema,
                                                    TipStanja = u.IdStanjeNavigation.TipStanja
                                                })
                                                .ToList()

                                })
                                .Skip((page - 1) * pagesize)
                                .Take(pagesize)
                                .ToListAsync();

            OpremaListViewModel olvm = new OpremaListViewModel()
            {
                OpremaList = opremaList,
                PagingInfo = pagingInfo
            };
            return View(olvm);
        }

        public async Task<IActionResult> Detalji(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = nameof(Detalji))
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            int pagesize = appSettings.PageSize;
            int pageOffset = appSettings.PageOffset;

            var oprema = await ctx.Oprema
                                .Select(t => new OpremaViewModel
                                {
                                    Id = t.Id,
                                    Redundantnost = t.Redundantnost,
                                    Budzet = t.Budzet,
                                    DatumPustanjaUPogon = t.DatumPustanjaUPogon,
                                    TipOpreme = t.IdTipOpremeNavigation.TipOpreme1,
                                    PodsustavNaziv = t.IdPodsustavNavigation.Naziv
                                })
                                .AsNoTracking()
                                .Where(o => o.Id == id)
                                .SingleOrDefaultAsync();

            if (oprema != null)
            {
                var query = ctx.Uredaj.AsNoTracking();
                int count = await query.CountAsync();

                var uredaji = await query.AsNoTracking()
                                .Where(u => u.IdOprema == id)
                                .Select(u => new UredajViewModel
                                {
                                    Id = u.Id,
                                    Naziv = u.Naziv,
                                    Proizvodac = u.Proizvodac,
                                    GodinaProizvodnje = u.GodinaProizvodnje,
                                    IdOprema = u.IdOprema,
                                    TipStanja = u.IdStanjeNavigation.TipStanja
                                })
                                .ToListAsync();
                oprema.Uredaji = uredaji;

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(viewName, oprema);
            }
            else
            {
                return NotFound($"Neispravan id opreme: {id}");
            }
        }

        public async Task<IActionResult> Sljedeci(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = "Detalji")
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            int pagesize = appSettings.PageSize;
            int pageOffset = appSettings.PageOffset;

            var query = ctx.Oprema.AsNoTracking();
            query = query.ApplySort(sort, ascending);

            var opremaList = await query
                                .Select(t => new OpremaViewModel
                                {
                                    Id = t.Id,
                                    Redundantnost = t.Redundantnost,
                                    Budzet = t.Budzet,
                                    DatumPustanjaUPogon = t.DatumPustanjaUPogon,
                                    TipOpreme = t.IdTipOpremeNavigation.TipOpreme1,
                                    PodsustavNaziv = t.IdPodsustavNavigation.Naziv
                                })
                                .AsNoTracking()
                                .ToListAsync();

            var stari = opremaList.Find(o => o.Id == id);
            int index = opremaList.IndexOf(stari);

            if (index != -1)
            {
                var oprema = opremaList[index + 1 < opremaList.Count ? index + 1 : opremaList.Count - 1];
                return RedirectToAction(viewName, new { id = oprema.Id, page, sort, ascending });
            }
            else
            {
                return NotFound($"Neispravan id opreme: {id}");
            }
        }

        public async Task<IActionResult> Prethodni(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = "Detalji")
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            int pagesize = appSettings.PageSize;
            int pageOffset = appSettings.PageOffset;


            var query = ctx.Oprema.AsNoTracking();
            query = query.ApplySort(sort, ascending);

            var opremaList = await query
                                .Select(t => new OpremaViewModel
                                {
                                    Id = t.Id,
                                    Redundantnost = t.Redundantnost,
                                    Budzet = t.Budzet,
                                    DatumPustanjaUPogon = t.DatumPustanjaUPogon,
                                    TipOpreme = t.IdTipOpremeNavigation.TipOpreme1,
                                    PodsustavNaziv = t.IdPodsustavNavigation.Naziv
                                })
                                .AsNoTracking()
                                .ToListAsync();

            var stari = opremaList.Find(o => o.Id == id);
            int index = opremaList.IndexOf(stari);
            if(index != -1)
            {
                var oprema = opremaList[index - 1 < 0 ? 0 : index - 1];
                return RedirectToAction(viewName, new { id = oprema.Id, page, sort, ascending });
            }
            else
            {
                return NotFound($"Neispravan id opreme: {id}");
            }

        }

        public IActionResult Dodaj()
        {
            ViewBag.Title = "Unos nove opreme";
            ViewBag.StudentTables = Constants.StudentTables;

            return View(new OpremaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(OpremaViewModel model)
        {
            ViewBag.Title = "Dodaj opremu";
            ViewBag.StudentTables = Constants.StudentTables;
            if (ModelState.IsValid)
            {
                Oprema o = new Oprema();
                o.Redundantnost = model.Redundantnost;
                o.Budzet = model.Budzet;
                o.DatumPustanjaUPogon = model.DatumPustanjaUPogon;
                o.IdPodsustav = (await ctx.Podsustav
                                .AsNoTracking()
                                .Where(t => t.Naziv == model.PodsustavNaziv)
                                .SingleOrDefaultAsync()).Id;
                o.IdTipOpreme = (await ctx.TipOpreme
                                .AsNoTracking()
                                .Where(t => t.TipOpreme1 == model.TipOpreme)
                                .SingleOrDefaultAsync()).Id;
                foreach (var uredaj in model.Uredaji)
                {
                    Uredaj noviUredaj = new Uredaj();
                    noviUredaj.Naziv = uredaj.Naziv;
                    noviUredaj.Proizvodac = uredaj.Proizvodac;
                    noviUredaj.GodinaProizvodnje = uredaj.GodinaProizvodnje;
                    noviUredaj.IdOprema = uredaj.IdOprema;
                    noviUredaj.IdStanje = (await ctx.Stanje
                                            .AsNoTracking()
                                            .Where(t => t.TipStanja == uredaj.TipStanja)
                                            .SingleOrDefaultAsync()).Id;
                    o.Uredaj.Add(noviUredaj);
                }

                try
                {
                    ctx.Add(o);
                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Oprema uspješno dodana. Id={o.Id}";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Uspješno dodana oprema. Id={id}", o.Id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    logger.LogError("Pogreška prilikom dodavanja nove opreme: {0}", exc.CompleteExceptionMessage());
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public Task<IActionResult> Uredi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            return Detalji(id, page, sort, ascending, viewName: nameof(Uredi));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(OpremaViewModel model, int page = 1, int sort = 1, bool ascending = true)
        {

            ViewBag.Title = "Uređivanje opreme";
            ViewBag.StudentTables = Constants.StudentTables;
            ViewBag.Page = page;
            ViewBag.Sort = sort;
            ViewBag.Ascending = ascending;
            if (ModelState.IsValid)
            {
                var oprema = await ctx.Oprema
                                .Include(o => o.Uredaj)
                                .Where(o => o.Id == model.Id)
                                .FirstOrDefaultAsync();

                if (oprema == null)
                {
                    return NotFound("Ne postoji oprema s id: " + model.Id);
                }
                oprema.Redundantnost = model.Redundantnost;
                oprema.Budzet = model.Budzet;
                oprema.DatumPustanjaUPogon = model.DatumPustanjaUPogon;
                oprema.IdPodsustav = (await ctx.Podsustav
                                .AsNoTracking()
                                .Where(t => t.Naziv == model.PodsustavNaziv)
                                .SingleOrDefaultAsync()).Id;
                oprema.IdTipOpreme = (await ctx.TipOpreme
                                .AsNoTracking()
                                .Where(t => t.TipOpreme1 == model.TipOpreme)
                                .SingleOrDefaultAsync()).Id;
                List<int> idUredaja = model.Uredaji
                                        .Where(u => u.Id > 0)
                                        .Select(u => u.Id)
                                        .ToList();
                ctx.RemoveRange(oprema.Uredaj.Where(u => !idUredaja.Contains(u.Id)));

                foreach (var uredaj in model.Uredaji)
                {
                    Uredaj noviUredaj;
                    if (uredaj.Id > 0)
                    {
                        noviUredaj = oprema.Uredaj.First(u => u.Id == uredaj.Id);
                    }
                    else
                    {
                        noviUredaj = new Uredaj();
                        oprema.Uredaj.Add(noviUredaj);
                    }
                    noviUredaj.Naziv = uredaj.Naziv;
                    noviUredaj.Proizvodac = uredaj.Proizvodac;
                    noviUredaj.GodinaProizvodnje = uredaj.GodinaProizvodnje;
                    noviUredaj.IdOprema = uredaj.IdOprema;
                    noviUredaj.IdStanje = (await ctx.Stanje
                                            .AsNoTracking()
                                            .Where(t => t.TipStanja == uredaj.TipStanja)
                                            .SingleOrDefaultAsync()).Id;
                }
                try
                {
                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Oprema {oprema.Id} uspješno ažurirana.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Oprema {Id} uspješno ažurirana.", oprema.Id);
                    return RedirectToAction(nameof(Uredi), new
                    {
                        id = oprema.Id,
                        page,
                        sort,
                        ascending
                    });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    logger.LogError("Pogreška prilikom uređivanja opreme: {0}", exc.CompleteExceptionMessage());
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }
        public async Task<IActionResult> Obrisi(int Id, int page = 1, int sort = 1, bool ascending = true)
        {
            var oprema = await ctx.Oprema
                                    .Where(o => o.Id == Id)
                                    .SingleOrDefaultAsync();
            if (oprema != null)
            {
                try
                {
                    ctx.Remove(oprema);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Oprema {oprema.Id} uspješno obrisana.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Oprema {id} uspješno obrisana.", oprema.Id);
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja opreme: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja opreme: {0}", exc.CompleteExceptionMessage());
                }
            }
            else
            {
                TempData[Constants.Message] = "Ne postoji oprema s id: " + Id;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
    }
}
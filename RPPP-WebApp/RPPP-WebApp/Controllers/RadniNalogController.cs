using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;
using System.Threading.Tasks;
using RPPP_WebApp.ViewModels;
using RPPP_WebApp.Extensions.Selectors;
using System.Linq;
using System;
using RPPP_WebApp.Extensions;
using System.Collections.Generic;

namespace RPPP_WebApp.Controllers
{
    public class RadniNalogController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<RadniNalogController> logger;
        private readonly AppSettings appSettings;

        public RadniNalogController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<RadniNalogController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            this.appSettings = options.Value;
        }
        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = "Radni nalog";
            ViewBag.StudentTables = Constants.StudentTables;

            int pagesize = appSettings.PageSize;
            int pageOffset = appSettings.PageOffset;
            var query = ctx.RadniNalog.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                TempData[Constants.Message] = "Ne postoji niti jedan radni nalog.";
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

            var radniNalozi = await query
                          .Select(r => new RadniNalogViewModel
                          {
                              Id = r.Id,
                              Sla = r.Sla,
                              Trajanje = r.Trajanje,
                              RadniListovi = r.RadniList
                                        .Select(r => new RadniListViewModel
                                        {
                                            Id = r.Id,
                                            PocetakRada = r.PocetakRada,
                                            TrajanjeRada = r.TrajanjeRada,
                                            OpisRada = r.OpisRada,
                                            NazivUredaja = r.IdUredajNavigation.Naziv,
                                            IdUredaj = r.IdUredaj,
                                            IdTimZaOdrzavanje = r.IdTimZaOdrzavanje,
                                            IdStatus = r.IdStatus,
                                            TimZaOdrzavanje = r.IdTimZaOdrzavanjeNavigation.NazivTima,
                                            Status = r.IdStatusNavigation.NazivStatusa
                                        })
                                        .ToList(),
                              TipRada = r.TipRada,
                              TragKvara = r.TragKvara,
                              PocetakRada = r.PocetakRada,
                              Kontrolor = r.IdKontrolorNavigation.KorisnickoIme,
                              Lokacija = r.IdLokacijaNavigation.Naziv,
                              Kvar = r.IdKvarNavigation.Opis,
                              Status = r.IdStatusNavigation.NazivStatusa,
                              StupanjPrioriteta = r.IdStupanjPrioritetaNavigation.StupanjPrioriteta,
                              IdStupanjPrioriteta = r.IdStupanjPrioriteta,
                              IdStatus = r.IdStatus,
                              IdKontrolor = r.IdKontrolor,
                              IdKvar = r.IdKvar,
                              IdLokacija = r.IdLokacija,
                              IdVoditeljSmjene = r.IdVoditeljSmjene
                          })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            RadniNaloziViewModel model = new RadniNaloziViewModel()
            {
                RadniNalozi = radniNalozi,
                PagingInfo = pagingInfo,
            };
            return View(model);
        }

        public async Task<IActionResult> Detalji(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = nameof(Detalji))
        {
            ViewBag.Title = "Radni nalog";
            ViewBag.StudentTables = Constants.StudentTables;


            int pagesize = appSettings.PageSize;
            int pageOffset = appSettings.PageOffset;

            var radniNalozi = await ctx.RadniNalog
                                  .Select(r => new RadniNalogViewModel
                                  {
                                      Id = r.Id,
                                      Sla = r.Sla,
                                      Trajanje = r.Trajanje,
                                      RadniListovi = r.RadniList
                                        .Select(r => new RadniListViewModel
                                        {
                                            Id = r.Id,
                                            PocetakRada = r.PocetakRada,
                                            TrajanjeRada = r.TrajanjeRada,
                                            OpisRada = r.OpisRada,
                                            NazivUredaja = r.IdUredajNavigation.Naziv,
                                            IdUredaj = r.IdUredaj,
                                            IdTimZaOdrzavanje = r.IdTimZaOdrzavanje,
                                            IdStatus = r.IdStatus,
                                            TimZaOdrzavanje = r.IdTimZaOdrzavanjeNavigation.NazivTima,
                                            Status = r.IdStatusNavigation.NazivStatusa
                                        })
                                        .ToList(),
                                      TipRada = r.TipRada,
                                      TragKvara = r.TragKvara,
                                      PocetakRada = r.PocetakRada,
                                      Kontrolor = r.IdKontrolorNavigation.KorisnickoIme,
                                      Lokacija = r.IdLokacijaNavigation.Naziv,
                                      Kvar = r.IdKvarNavigation.Opis,
                                      Status = r.IdStatusNavigation.NazivStatusa,
                                      StupanjPrioriteta = r.IdStupanjPrioritetaNavigation.StupanjPrioriteta,
                                      IdStupanjPrioriteta = r.IdStupanjPrioriteta,
                                      IdStatus = r.IdStatus,
                                      IdKontrolor = r.IdKontrolor,
                                      IdKvar = r.IdKvar,
                                      IdLokacija = r.IdLokacija,
                                      IdVoditeljSmjene = r.IdVoditeljSmjene
                                  })
                                  .AsNoTracking()
                                  .Where(r => r.Id == id)
                                  .SingleOrDefaultAsync();

            if (radniNalozi != null)
            {
                var query = ctx.RadniList.AsNoTracking();

                int count = await query.CountAsync();

                var radniListovi = await query.AsNoTracking()
                    .Where(r => r.IdRadniNalog == id)
                    .Select(r => new RadniListViewModel
                    {
                        Id = r.Id,
                        PocetakRada = r.PocetakRada,
                        TrajanjeRada = r.TrajanjeRada,
                        OpisRada = r.OpisRada,
                        NazivUredaja = r.IdUredajNavigation.Naziv,
                        IdUredaj = r.IdUredaj,
                        IdTimZaOdrzavanje = r.IdTimZaOdrzavanje,
                        IdStatus = r.IdStatus,
                        TimZaOdrzavanje = r.IdTimZaOdrzavanjeNavigation.NazivTima,
                        Status = r.IdStatusNavigation.NazivStatusa
                    })
                    .ToListAsync();

                radniNalozi.RadniListovi = radniListovi;

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(viewName, radniNalozi);
            }
            else
            {
                return NotFound($"Neispravan id radnog naloga {id}");
            }
        }

        public IActionResult Dodaj()
        {
            ViewBag.Title = "Unos novog radnog naloga";
            ViewBag.StudentTables = Constants.StudentTables;

            return View(new RadniNalogViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(RadniNalogViewModel model)
        {
            ViewBag.Title = "Dodaj radni nalog";
            ViewBag.StudentTables = Constants.StudentTables;
            if (ModelState.IsValid)
            {
                RadniNalog r = new RadniNalog();
                r.Sla = model.Sla;
                r.Trajanje = model.Trajanje;
                r.TipRada = model.TipRada;
                r.TragKvara = model.TragKvara;
                r.PocetakRada = model.PocetakRada;
                r.IdKontrolor = model.IdKontrolor;
                r.IdVoditeljSmjene = 1;
                r.IdLokacija = model.IdLokacija;
                r.IdKvar = model.IdKvar;
                r.IdStatus = model.IdStatus;
                r.IdStupanjPrioriteta = model.IdStupanjPrioriteta;
                foreach (var radniList in model.RadniListovi)
                {
                    RadniList noviRadniList = new RadniList();
                    noviRadniList.PocetakRada = radniList.PocetakRada;
                    noviRadniList.TrajanjeRada = radniList.TrajanjeRada;
                    noviRadniList.OpisRada = radniList.OpisRada;
                    noviRadniList.IdUredaj = radniList.IdUredaj;
                    noviRadniList.IdTimZaOdrzavanje = radniList.IdTimZaOdrzavanje;
                    noviRadniList.IdStatus = radniList.IdStatus;
                    r.RadniList.Add(noviRadniList);
                }

                try
                {
                    ctx.Add(r);
                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Radni nalog uspješno dodan. Id={r.Id}";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Uspješno dodan radni nalog. Id={id}", r.Id);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    logger.LogError("Pogreška prilikom dodavanja novog radnog naloga: {0}", exc.CompleteExceptionMessage());
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
        public async Task<IActionResult> Uredi(RadniNalogViewModel model, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = "Uredivanje radnog naloga";
            ViewBag.StudentTables = Constants.StudentTables;
            ViewBag.Page = page;
            ViewBag.Sort = sort;
            ViewBag.Ascending = ascending;
            if (ModelState.IsValid)
            {
                var radniNalog = await ctx.RadniNalog
                                        .Include(d => d.RadniList)
                                        .Where(d => d.Id == model.Id)
                                        .FirstOrDefaultAsync();
                if (radniNalog == null)
                {
                    return NotFound("Ne postoji radni nalog s id-om: " + model.Id);
                }

                radniNalog.Sla = model.Sla;
                radniNalog.Trajanje = model.Trajanje;
                radniNalog.TipRada = model.TipRada;
                radniNalog.TragKvara = model.TragKvara;
                radniNalog.PocetakRada = model.PocetakRada;
                radniNalog.IdKontrolor = model.IdKontrolor;
                radniNalog.IdStupanjPrioriteta = model.IdStupanjPrioriteta;
                radniNalog.IdLokacija = model.IdLokacija;
                radniNalog.IdKvar = model.IdKvar;
                radniNalog.IdStatus = model.IdStatus;
                List<int> idRadnihListova = model.RadniListovi
                                          .Where(r => r.Id > 0)
                                          .Select(r => r.Id)
                                          .ToList();
                ctx.RemoveRange(radniNalog.RadniList.Where(r => !idRadnihListova.Contains(r.Id)));

                foreach (var radniList in model.RadniListovi)
                {
                    //ažuriraj postojeće i dodaj nove
                    RadniList noviRadniList; // potpuno nova ili dohvaćena ona koju treba izmijeniti
                    if (radniList.Id > 0)
                    {
                        noviRadniList = radniNalog.RadniList.First(r => r.Id == radniList.Id);
                    }
                    else
                    {
                        noviRadniList = new RadniList();
                        radniNalog.RadniList.Add(noviRadniList);
                    }
                    noviRadniList.PocetakRada = radniList.PocetakRada;
                    noviRadniList.TrajanjeRada = radniList.TrajanjeRada;
                    noviRadniList.OpisRada = radniList.OpisRada;
                    noviRadniList.IdUredaj = radniList.IdUredaj;
                    noviRadniList.IdTimZaOdrzavanje = radniList.IdTimZaOdrzavanje;
                    noviRadniList.IdStatus = radniList.IdStatus;
                }
                try
                {

                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Radni nalog {radniNalog.Id} uspješno ažuriran.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Radni nalog {Id} uspješno ažuriran.", radniNalog.Id);
                    return RedirectToAction(nameof(Uredi), new
                    {
                        id = radniNalog.Id,
                        page,
                        sort,
                        ascending
                    });

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    logger.LogError("Pogreška prilikom uredivanja radnog naloga: {0}", exc.CompleteExceptionMessage());
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
            var radniNalog = await ctx.RadniNalog
                                    .Where(t => t.Id == Id)
                                    .SingleOrDefaultAsync();
            if (radniNalog != null)
            {
                try
                {
                    ctx.Remove(radniNalog);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Radni nalog {radniNalog.Id} uspješno obrisan.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Radni nalog {id} uspješno obrisan.", radniNalog.Id);
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja radnog naloga: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja radnog naloga: {0}", exc.CompleteExceptionMessage());
                }
            }
            else
            {
                TempData[Constants.Message] = "Ne postoji radni nalog s id-om: " + Id;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }

        public async Task<IActionResult> Sljedeci(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = "Detalji")
        {
            ViewBag.Title = "Radni nalog";
            ViewBag.StudentTables = Constants.StudentTables;


            int pagesize = appSettings.PageSize;
            int pageOffset = appSettings.PageOffset;

            var query = ctx.RadniNalog.AsNoTracking();
            query = query.ApplySort(sort, ascending);

            var radniNalozi = await query
                                  .Select(r => new RadniNalogViewModel
                                  {
                                      Id = r.Id,
                                      Sla = r.Sla,
                                      Trajanje = r.Trajanje,
                                      TipRada = r.TipRada,
                                      TragKvara = r.TragKvara,
                                      PocetakRada = r.PocetakRada,
                                      Kontrolor = r.IdKontrolorNavigation.KorisnickoIme,
                                      Lokacija = r.IdLokacijaNavigation.Naziv,
                                      Kvar = r.IdKvarNavigation.Opis,
                                      Status = r.IdStatusNavigation.NazivStatusa,
                                      StupanjPrioriteta = r.IdStupanjPrioritetaNavigation.StupanjPrioriteta,
                                      IdStupanjPrioriteta = r.IdStupanjPrioriteta,
                                      IdStatus = r.IdStatus,
                                      IdKontrolor = r.IdKontrolor,
                                      IdKvar = r.IdKvar,
                                      IdLokacija = r.IdLokacija,
                                      IdVoditeljSmjene = r.IdVoditeljSmjene
                                  })
                                  .AsNoTracking()
                                  .ToListAsync();
            var stari = radniNalozi.Find(r => r.Id == id);
            int index = radniNalozi.IndexOf(stari);

            if (index != -1)
            {
                var radniNalog = radniNalozi[index + 1 < radniNalozi.Count ? index + 1 : radniNalozi.Count - 1];

                return RedirectToAction(viewName, new { id = radniNalog.Id, page, sort, ascending });
            }
            else
            {
                return NotFound($"Neispravan id radnog naloga {id}");
            }
        }

        public async Task<IActionResult> Prethodni(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = "Detalji")
        {
            ViewBag.Title = "Radni nalog";
            ViewBag.StudentTables = Constants.StudentTables;


            int pagesize = appSettings.PageSize;
            int pageOffset = appSettings.PageOffset;

            var query = ctx.RadniNalog.AsNoTracking();
            query = query.ApplySort(sort, ascending);

            var radniNalozi = await query
                                  .Select(r => new RadniNalogViewModel
                                  {
                                      Id = r.Id,
                                      Sla = r.Sla,
                                      Trajanje = r.Trajanje,
                                      TipRada = r.TipRada,
                                      TragKvara = r.TragKvara,
                                      PocetakRada = r.PocetakRada,
                                      Kontrolor = r.IdKontrolorNavigation.KorisnickoIme,
                                      Lokacija = r.IdLokacijaNavigation.Naziv,
                                      Kvar = r.IdKvarNavigation.Opis,
                                      Status = r.IdStatusNavigation.NazivStatusa,
                                      StupanjPrioriteta = r.IdStupanjPrioritetaNavigation.StupanjPrioriteta,
                                      IdStupanjPrioriteta = r.IdStupanjPrioriteta,
                                      IdStatus = r.IdStatus,
                                      IdKontrolor = r.IdKontrolor,
                                      IdKvar = r.IdKvar,
                                      IdLokacija = r.IdLokacija,
                                      IdVoditeljSmjene = r.IdVoditeljSmjene
                                  })
                                  .AsNoTracking().ToListAsync();

            var stari = radniNalozi.Find(r => r.Id == id);
            int index = radniNalozi.IndexOf(stari);

            if (index != -1)
            {
                var radniNalog = radniNalozi[index - 1 < 0 ? 0 : index - 1];

                return RedirectToAction(viewName, new { id = radniNalog.Id, page, sort, ascending });
            }
            else
            {
                return NotFound($"Neispravan id radnog naloga {id}");
            }
        }
    }
}

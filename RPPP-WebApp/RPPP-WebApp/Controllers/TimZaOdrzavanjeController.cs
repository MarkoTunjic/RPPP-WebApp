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
    public class TimZaOdrzavanjeController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<TimZaOdrzavanjeController> logger;
        private readonly AppSettings appSettings;

        public TimZaOdrzavanjeController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<TimZaOdrzavanjeController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            this.appSettings = options.Value;
        }
        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = "Tim za održavanje";
            ViewBag.StudentTables = Constants.StudentTables;

            int pagesize = appSettings.PageSize;
            int pageOffset = appSettings.PageOffset;
            var query = ctx.TimZaOdrzavanje.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                TempData[Constants.Message] = "Ne postoji niti jedan tim za održavanje.";
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

            var timoviZaOdrzavanje = await query
                          .Select(t => new TimZaOdrzavanjeViewModel
                          {
                              Id = t.Id,
                              NazivTima = t.NazivTima,
                              DatumOsnivanja = t.DatumOsnivanja,
                              Radnici = t.Radnik
                                        .Select(r => new RadnikViewModel
                                        {
                                            Certifikat = r.Certifikat,
                                            Dezuran = r.Dezuran,
                                            Id = r.Id,
                                            Ime = r.Ime,
                                            Prezime = r.Prezime,
                                            IstekCertifikata = r.IstekCertifikata,
                                            IdStrucnaSprema = r.IdStrucnaSprema,
                                            RazinaStrucneSpreme = r.IdStrucnaSpremaNavigation.RazinaStrucneSpreme
                                        })
                                        .ToList(),
                              Satnica = t.Satnica,
                              IdPodrucjaRada = t.IdPodrucjeRada,
                              VrstaPodrucjaRada = t.IdPodrucjeRadaNavigation.VrstaPodrucjaRada
                          })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            TimoviZaOdrzavanjeViewModel model = new TimoviZaOdrzavanjeViewModel()
            {
                Timovi = timoviZaOdrzavanje,
                PagingInfo = pagingInfo,
            };
            return View(model);
        }

        public async Task<IActionResult> Detalji(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = nameof(Detalji))
        {
            ViewBag.Title = "Tim za održavanje";
            ViewBag.StudentTables = Constants.StudentTables;

            var timZaOdrzavanje = await ctx.TimZaOdrzavanje
                                  .Select(t => new TimZaOdrzavanjeViewModel
                                  {
                                      Id = t.Id,
                                      NazivTima = t.NazivTima,
                                      DatumOsnivanja = t.DatumOsnivanja,
                                      Satnica = t.Satnica,
                                      IdPodrucjaRada = t.IdPodrucjeRada,
                                      VrstaPodrucjaRada = t.IdPodrucjeRadaNavigation.VrstaPodrucjaRada
                                  })
                                  .AsNoTracking()
                                  .Where(t => t.Id == id)
                                  .SingleOrDefaultAsync();

            if (timZaOdrzavanje != null)
            {
                var query = ctx.Radnik.AsNoTracking();

                var radnici = await query.AsNoTracking()
                    .Where(r => r.IdTimZaOdrzavanje == id)
                    .Select(r => new RadnikViewModel
                    {
                        Certifikat = r.Certifikat,
                        Dezuran = r.Dezuran,
                        Id = r.Id,
                        Ime = r.Ime,
                        Prezime = r.Prezime,
                        IstekCertifikata = r.IstekCertifikata,
                        IdStrucnaSprema = r.IdStrucnaSprema,
                        RazinaStrucneSpreme = r.IdStrucnaSpremaNavigation.RazinaStrucneSpreme
                    })
                    .ToListAsync();

                timZaOdrzavanje.Radnici = radnici;

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(viewName, timZaOdrzavanje);
            }
            else
            {
                return NotFound($"Neispravan id tima za održavanje {id}");
            }
        }

        public IActionResult Dodaj()
        {
            ViewBag.Title = "Unos novog tima";
            ViewBag.StudentTables = Constants.StudentTables;

            return View(new TimZaOdrzavanjeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(TimZaOdrzavanjeViewModel model)
        {
            ViewBag.Title = "Dodaj tim za održavanje";
            ViewBag.StudentTables = Constants.StudentTables;
            if (ModelState.IsValid)
            {
                TimZaOdrzavanje t = new TimZaOdrzavanje();
                t.Satnica = model.Satnica;
                t.NazivTima = model.NazivTima;
                t.IdPodrucjeRada = model.IdPodrucjaRada;
                t.DatumOsnivanja = model.DatumOsnivanja;
                foreach (var radnik in model.Radnici)
                {
                    Radnik noviRadnik = new Radnik();
                    noviRadnik.IdStrucnaSprema = radnik.IdStrucnaSprema;
                    noviRadnik.Ime = radnik.Ime;
                    noviRadnik.Prezime = radnik.Prezime;
                    noviRadnik.Certifikat = radnik.Certifikat;
                    noviRadnik.IstekCertifikata = radnik.IstekCertifikata;
                    noviRadnik.Dezuran = radnik.Dezuran;
                    t.Radnik.Add(noviRadnik);
                }

                try
                {
                    ctx.Add(t);
                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Tim uspješno dodan. Id={t.Id}";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Uspješno dodan tim. Id={id}", t.Id);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    logger.LogError(exc, "Pogreška prilikom dodavanja novog tima za održavanje: {0}", exc.CompleteExceptionMessage());
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
        public async Task<IActionResult> Uredi(TimZaOdrzavanjeViewModel model, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = "Uređivanje tima za održavanje";
            ViewBag.StudentTables = Constants.StudentTables;
            ViewBag.Page = page;
            ViewBag.Sort = sort;
            ViewBag.Ascending = ascending;
            if (ModelState.IsValid)
            {
                var timZaOdrzavanje = await ctx.TimZaOdrzavanje
                                        .Include(d => d.Radnik)
                                        .Where(d => d.Id == model.Id)
                                        .FirstOrDefaultAsync();
                if (timZaOdrzavanje == null)
                {
                    return NotFound("Ne postoji tim s id-om: " + model.Id);
                }

                timZaOdrzavanje.Satnica = model.Satnica;
                timZaOdrzavanje.NazivTima = model.NazivTima;
                timZaOdrzavanje.IdPodrucjeRada = model.IdPodrucjaRada;
                timZaOdrzavanje.DatumOsnivanja = model.DatumOsnivanja;
                List<int> idRadnika = model.Radnici
                                          .Where(r => r.Id > 0)
                                          .Select(r => r.Id)
                                          .ToList();
                ctx.RemoveRange(timZaOdrzavanje.Radnik.Where(r => !idRadnika.Contains(r.Id)));

                foreach (var radnik in model.Radnici)
                {
                    //ažuriraj postojeće i dodaj nove
                    Radnik noviRadnik; // potpuno nova ili dohvaćena ona koju treba izmijeniti
                    if (radnik.Id > 0)
                    {
                        noviRadnik = timZaOdrzavanje.Radnik.First(r => r.Id == radnik.Id);
                    }
                    else
                    {
                        noviRadnik = new Radnik();
                        timZaOdrzavanje.Radnik.Add(noviRadnik);
                    }
                    noviRadnik.IdStrucnaSprema = radnik.IdStrucnaSprema;
                    noviRadnik.Ime = radnik.Ime;
                    noviRadnik.Prezime = radnik.Prezime;
                    noviRadnik.Certifikat = radnik.Certifikat;
                    noviRadnik.IstekCertifikata = radnik.IstekCertifikata;
                    noviRadnik.Dezuran = radnik.Dezuran;
                }
                try
                {

                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Tim {timZaOdrzavanje.Id} uspješno ažuriran.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Tim {Id} uspješno ažuriran.", timZaOdrzavanje.Id);
                    return RedirectToAction(nameof(Uredi), new
                    {
                        id = timZaOdrzavanje.Id,
                        page,
                        sort,
                        ascending
                    });

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    logger.LogError(exc, "Pogreška prilikom uređivanja tima za održavanje: {0}", exc.CompleteExceptionMessage());
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
            var tim = await ctx.TimZaOdrzavanje
                                    .Where(t => t.Id == Id)
                                    .SingleOrDefaultAsync();
            if (tim != null)
            {
                try
                {
                    ctx.Remove(tim);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Tim {tim.Id} uspješno obrisan.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Tim {id} uspješno obrisan.", tim.Id);
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja tima: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError(exc, "Pogreška prilikom brisanja tima za održavanje: {0}", exc.CompleteExceptionMessage());
                }
            }
            else
            {
                TempData[Constants.Message] = "Ne postoji tim s id-om: " + Id;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }

        public async Task<IActionResult> Sljedeci(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = "Detalji")
        {
            ViewBag.Title = "Tim za održavanje";
            ViewBag.StudentTables = Constants.StudentTables;

            var query = ctx.TimZaOdrzavanje.AsNoTracking();
            query = query.ApplySort(sort, ascending);

            var timoviZaOdrzavanje = await query
                                  .Select(t => new TimZaOdrzavanjeViewModel
                                  {
                                      Id = t.Id,
                                      NazivTima = t.NazivTima,
                                      DatumOsnivanja = t.DatumOsnivanja,
                                      Satnica = t.Satnica,
                                      IdPodrucjaRada = t.IdPodrucjeRada,
                                      VrstaPodrucjaRada = t.IdPodrucjeRadaNavigation.VrstaPodrucjaRada
                                  })
                                  .AsNoTracking()
                                  .ToListAsync();

            var stari = timoviZaOdrzavanje.Find(t => t.Id == id);
            int index = timoviZaOdrzavanje.IndexOf(stari);

            if (index != -1)
            {
                var timZaOdrzavanje = timoviZaOdrzavanje[index + 1 < timoviZaOdrzavanje.Count ? index + 1 : timoviZaOdrzavanje.Count - 1];

                return RedirectToAction(viewName, new { id = timZaOdrzavanje.Id, page, sort, ascending });
            }
            else
            {
                return NotFound($"Neispravan id tima za održavanje {id}");
            }
        }

        public async Task<IActionResult> Prethodni(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = "Detalji")
        {
            ViewBag.Title = "Tim za održavanje";
            ViewBag.StudentTables = Constants.StudentTables;

            var query = ctx.TimZaOdrzavanje.AsNoTracking();
            query = query.ApplySort(sort, ascending);

            var timoviZaOdrzavanje = await query
                                  .Select(t => new TimZaOdrzavanjeViewModel
                                  {
                                      Id = t.Id,
                                      NazivTima = t.NazivTima,
                                      DatumOsnivanja = t.DatumOsnivanja,
                                      Satnica = t.Satnica,
                                      IdPodrucjaRada = t.IdPodrucjeRada,
                                      VrstaPodrucjaRada = t.IdPodrucjeRadaNavigation.VrstaPodrucjaRada
                                  })
                                  .AsNoTracking().ToListAsync();

            var stari = timoviZaOdrzavanje.Find(t => t.Id == id);
            int index = timoviZaOdrzavanje.IndexOf(stari);

            if (index != -1)
            {
                var timZaOdrzavanje = timoviZaOdrzavanje[index - 1 < 0 ? 0 : index - 1];

                return RedirectToAction(viewName, new { id = timZaOdrzavanje.Id, page, sort, ascending });
            }
            else
            {
                return NotFound($"Neispravan id tima za održavanje {id}");
            }
        }
    }
}

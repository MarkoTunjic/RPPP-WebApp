using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using RPPP_WebApp.Extensions;

namespace RPPP_WebApp.Controllers
{
    public class SmjenaController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<SmjenaController> logger;
        private readonly AppSettings appSettings;

        public SmjenaController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<SmjenaController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            this.appSettings = options.Value;
        }
        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = "Smjena";
            ViewBag.StudentTables = Constants.StudentTables;

            int pagesize = appSettings.PageSize;
            int pageOffset = appSettings.PageOffset;
            var query = ctx.Smjena.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                TempData[Constants.Message] = "Ne postoji niti jedna smjena.";
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

            var smjene = await query
                          .Select(s => new SmjenaViewModel
                          {
                              Id = s.Id,
                              PlatniFaktor = s.PlatniFaktor,
                              PocetakSmjene = s.PocetakSmjene,
                              Kontrolori = s.Kontrolor
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
                                            PocetakSmjene = k.IdSmjenaNavigation.PocetakSmjene,
                                            IdRang = k.IdRang
        })
                                        .ToList(),
                              VrijemeKrajaSmjene = s.IdKrajSmjeneNavigation.VrijemeKrajaSmjene,
                              IdKrajSmjene = s.IdKrajSmjene
                          })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            SmjeneViewModel model = new SmjeneViewModel()
            {
                Smjene = smjene,
                PagingInfo = pagingInfo,
            };
            return View(model);
        }

        public async Task<IActionResult> Detalji(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = nameof(Detalji))
        {
            ViewBag.Title = "Smjena";
            ViewBag.StudentTables = Constants.StudentTables;

            var smjena = await ctx.Smjena
                                  .Select(s => new SmjenaViewModel
                                  {
                                      Id = s.Id,
                                      PocetakSmjene = s.PocetakSmjene,
                                      PlatniFaktor = s.PlatniFaktor,
                                      VrijemeKrajaSmjene = s.IdKrajSmjeneNavigation.VrijemeKrajaSmjene,
                                      IdKrajSmjene = s.IdKrajSmjene
                                  })
                                  .AsNoTracking()
                                  .Where(s => s.Id == id)
                                  .SingleOrDefaultAsync();

            if (smjena != null)
            {
                var query = ctx.Kontrolor.AsNoTracking();

                var kontrolori = await query.AsNoTracking()
                    .Where(k => k.IdSmjena == id)
                    .Select(k => new KontrolorViewModel
                    {
                        Id = k.Id,
                        Ime = k.Ime,
                        Prezime = k.Prezime,
                        Oib = k.Oib,
                        DatumZaposlenja = k.DatumZaposlenja,
                        ZaposlenDo = k.ZaposlenDo,
                        Lozinka = k.Lozinka,
                        KorisnickoIme = k.KorisnickoIme,
                        ImeRanga = k.IdRangNavigation.ImeRanga,
                        PocetakSmjene = k.IdSmjenaNavigation.PocetakSmjene,
                        IdRang = k.IdRang
                    })
                    .ToListAsync();

                smjena.Kontrolori = kontrolori;

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(viewName, smjena);
            }
            else
            {
                return NotFound($"Neispravan id smjene {id}");
            }
        }

        public async Task<IActionResult> Sljedeci(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = "Detalji")
        {
            ViewBag.Title = "Smjena";
            ViewBag.StudentTables = Constants.StudentTables;

            var query = ctx.Smjena.AsNoTracking();
            query = query.ApplySort(sort, ascending);

            var smjene = await query
                                  .Select(s => new SmjenaViewModel
                                  {
                                      Id = s.Id,
                                      PocetakSmjene = s.PocetakSmjene,
                                      PlatniFaktor = s.PlatniFaktor,
                                      VrijemeKrajaSmjene = s.IdKrajSmjeneNavigation.VrijemeKrajaSmjene,
                                      IdKrajSmjene = s.IdKrajSmjene
                                  })
                                  .AsNoTracking()
                                  .ToListAsync();

            var stari = smjene.Find(s => s.Id == id);
            int index = smjene.IndexOf(stari);

            if (index != -1)
            {
                var smjena = smjene[index + 1 < smjene.Count ? index + 1 : smjene.Count - 1];

                return RedirectToAction(viewName, new { id = smjena.Id, page, sort, ascending });
            }
            else
            {
                return NotFound($"Neispravan id smjene {id}");
            }
        }

        public async Task<IActionResult> Prethodni(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = "Detalji")
        {
            ViewBag.Title = "Smjena";
            ViewBag.StudentTables = Constants.StudentTables;

            var query = ctx.Smjena.AsNoTracking();
            query = query.ApplySort(sort, ascending);

            var smjene = await query
                                  .Select(s => new SmjenaViewModel
                                  {
                                      Id = s.Id,
                                      PocetakSmjene = s.PocetakSmjene,
                                      PlatniFaktor = s.PlatniFaktor,
                                      VrijemeKrajaSmjene = s.IdKrajSmjeneNavigation.VrijemeKrajaSmjene,
                                      IdKrajSmjene = s.IdKrajSmjene
                                  })
                                  .AsNoTracking().ToListAsync();

            var stari = smjene.Find(s => s.Id == id);
            int index = smjene.IndexOf(stari);

            if (index != -1)
            {
                var smjena = smjene[index - 1 < 0 ? 0 : index - 1];

                return RedirectToAction(viewName, new { id = smjena.Id, page, sort, ascending });
            }
            else
            {
                return NotFound($"Neispravan id smjene {id}");
            }
        }
        
         public IActionResult Dodaj()
        {
            ViewBag.Title = "Unos nove smjene";
            ViewBag.StudentTables = Constants.StudentTables;

            return View(new SmjenaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(SmjenaViewModel model)
        {
            ViewBag.Title = "Dodaj smjenu";
            ViewBag.StudentTables = Constants.StudentTables;
            if (ModelState.IsValid)
            {
                Smjena s = new Smjena();
                s.Id = model.Id;
                s.PocetakSmjene = model.PocetakSmjene;
                s.PlatniFaktor = model.PlatniFaktor;
                s.IdKrajSmjene = model.IdKrajSmjene;
                foreach (var kontrolor in model.Kontrolori)
                {
                    Kontrolor noviKontrolor = new Kontrolor();
                    noviKontrolor.Id = kontrolor.Id;
                    noviKontrolor.Ime = kontrolor.Ime;
                    noviKontrolor.Prezime = kontrolor.Prezime;
                    noviKontrolor.Oib = kontrolor.Oib;
                    noviKontrolor.DatumZaposlenja = kontrolor.DatumZaposlenja;
                    noviKontrolor.ZaposlenDo = kontrolor.ZaposlenDo;
                    noviKontrolor.Lozinka = kontrolor.Lozinka;
                    noviKontrolor.KorisnickoIme = kontrolor.KorisnickoIme;
                    noviKontrolor.IdRang = kontrolor.IdRang;
                    s.Kontrolor.Add(noviKontrolor);
                }

                try
                {
                    ctx.Add(s);
                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Smjena uspješno dodana. Id={s.Id}";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Uspješno dodana smjena. Id={id}", s.Id);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    logger.LogError("Pogreška prilikom dodavanja nove smjene: {0}", exc.CompleteExceptionMessage());
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
        public async Task<IActionResult> Uredi(SmjenaViewModel model, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = "Uredivanje smjene";
            ViewBag.StudentTables = Constants.StudentTables;
            ViewBag.Page = page;
            ViewBag.Sort = sort;
            ViewBag.Ascending = ascending;
            if (ModelState.IsValid)
            {
                var smjena = await ctx.Smjena
                                        .Include(s => s.Kontrolor)
                                        .Where(s => s.Id == model.Id)
                                        .FirstOrDefaultAsync();
                if (smjena == null)
                {
                    return NotFound("Ne postoji smjena s id-om: " + model.Id);
                }

                smjena.Id = model.Id;
                smjena.PocetakSmjene = model.PocetakSmjene;
                smjena.PlatniFaktor = model.PlatniFaktor;
                smjena.IdKrajSmjene = model.IdKrajSmjene;
                List<int> idKontrolora = model.Kontrolori
                                          .Where(k => k.Id > 0)
                                          .Select(k => k.Id)
                                          .ToList();
                ctx.RemoveRange(smjena.Kontrolor.Where(k => !idKontrolora.Contains(k.Id)));

                foreach (var kontrolor in model.Kontrolori)
                {
                    //ažuriraj postojeće i dodaj nove
                    Kontrolor noviKontrolor; // potpuno nova ili dohvaćena ona koju treba izmijeniti
                    if (kontrolor.Id > 0)
                    {
                        noviKontrolor = smjena.Kontrolor.First(k => k.Id == kontrolor.Id);
                    }
                    else
                    {
                        noviKontrolor = new Kontrolor();
                        smjena.Kontrolor.Add(noviKontrolor);
                    }
                    noviKontrolor.Id = kontrolor.Id;
                    noviKontrolor.Ime = kontrolor.Ime;
                    noviKontrolor.Prezime = kontrolor.Prezime;
                    noviKontrolor.Oib = kontrolor.Oib;
                    noviKontrolor.DatumZaposlenja = kontrolor.DatumZaposlenja;
                    noviKontrolor.ZaposlenDo = kontrolor.ZaposlenDo;
                    noviKontrolor.Lozinka = kontrolor.Lozinka;
                    noviKontrolor.KorisnickoIme = kontrolor.KorisnickoIme;
                    noviKontrolor.IdRang = kontrolor.IdRang;
                }
                try
                {

                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Smjena {smjena.Id} uspješno ažurirana.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Smjena {Id} uspješno ažurirana.", smjena.Id);
                    return RedirectToAction(nameof(Uredi), new
                    {
                        id = smjena.Id,
                        page,
                        sort,
                        ascending
                    });

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    logger.LogError("Pogreška prilikom uređivanja smjene: {0}", exc.CompleteExceptionMessage());
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
            var smjena = await ctx.Smjena
                                    .Where(s => s.Id == Id)
                                    .SingleOrDefaultAsync();
            if (smjena != null)
            {
                try
                {
                    ctx.Remove(smjena);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Smjena {smjena.Id} uspješno obrisana.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Smjena {id} uspješno obrisana.", smjena.Id);
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja smjene: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja smjene: {0}", exc.CompleteExceptionMessage());
                }
            }
            else
            {
                TempData[Constants.Message] = "Ne postoji smjena s id-om: " + Id;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
    }
}

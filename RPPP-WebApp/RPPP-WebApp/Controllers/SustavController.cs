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
    public class SustavController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<SustavController> logger;
        private readonly AppSettings appSettings;

        public SustavController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<SustavController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            this.appSettings = options.Value;
        }
        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = "Sustav";
            ViewBag.StudentTables = Constants.StudentTables;

            int pagesize = appSettings.PageSize;
            int pageOffset = appSettings.PageOffset;
            var query = ctx.Sustav.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                string message = "Ne postoji niti jedan sustav!";
                logger.LogInformation(message);
                TempData[Constants.Message] = message;
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

            var sustavi = await query
                          .Select(s => new SustavViewModel
                          {
                              Id = s.Id,
                              Opis = s.Opis,
                              Osjetljivost = s.Osjetljivost,
                              IdStupanjKriticnosti = s.IdKriticnostNavigation.Id,
                              StupanjKriticnosti = s.IdKriticnostNavigation.StupanjKriticnosti,
                              IdVrstaSustava = s.IdVrstaSustavaNavigation.Id,
                              VrstaSustava = s.IdVrstaSustavaNavigation.NazivVrsteSustava,
                              Podsustavi = s.Podsustav
                                           .Select(pod => new PodsustavViewModel
                                           {
                                               Id = pod.Id,
                                               UcestalostOdrzavanja = pod.UcestalostOdrzavanja,
                                               Osjetljivost = pod.Osjetljivost,
                                               Naziv = pod.Naziv,
                                               Opis = pod.Opis,
                                               OpisSustava = pod.IdSustavNavigation.Opis,
                                               NazivLokacije = pod.IdLokacijaNavigation.Naziv,
                                               IdStupanjKriticnosti = pod.IdKriticnostNavigation.Id,
                                               StupanjKriticnosti = pod.IdKriticnostNavigation.StupanjKriticnosti,

                                           })
                                           .ToList(),


                          }).Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            SustaviViewModel model = new SustaviViewModel()
            {
                Sustavi = sustavi,
                PagingInfo = pagingInfo,
            };
            return View(model);
        }

        public async Task<IActionResult> Detalji(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = nameof(Detalji))
        {
            ViewBag.Title = "Sustav";
            ViewBag.StudentTables = Constants.StudentTables;


            int pagesize = appSettings.PageSize;
            int pageOffset = appSettings.PageOffset;

            var sustav = await ctx.Sustav
                                  .Select(s => new SustavViewModel
                                  {
                                      Id = s.Id,
                                      Opis = s.Opis,
                                      Osjetljivost = s.Osjetljivost,
                                      IdStupanjKriticnosti = s.IdKriticnostNavigation.Id,
                                      StupanjKriticnosti = s.IdKriticnostNavigation.StupanjKriticnosti,
                                      IdVrstaSustava = s.IdVrstaSustavaNavigation.Id,
                                      VrstaSustava = s.IdVrstaSustavaNavigation.NazivVrsteSustava,

                                  })
                                  .AsNoTracking()
                                  .Where(t => t.Id == id)
                                  .SingleOrDefaultAsync();

            if (sustav != null)
            {
                var query = ctx.Podsustav.AsNoTracking();

                

                int count = await query.CountAsync();

                var podsustavi = await query.AsNoTracking()
                    .Where(r => r.IdSustav == id)
                    .Select(pod => new PodsustavViewModel
                    {
                        Id = pod.Id,
                        UcestalostOdrzavanja = pod.UcestalostOdrzavanja,
                        Osjetljivost = pod.Osjetljivost,
                        Naziv = pod.Naziv,
                        Opis = pod.Opis,
                        OpisSustava = pod.IdSustavNavigation.Opis,
                        IdLokacija = pod.IdLokacijaNavigation.Id,
                        NazivLokacije = pod.IdLokacijaNavigation.Naziv,
                        IdStupanjKriticnosti = pod.IdKriticnostNavigation.Id,
                        StupanjKriticnosti = pod.IdKriticnostNavigation.StupanjKriticnosti,
                    })
                    .ToListAsync();

                sustav.Podsustavi = podsustavi;

                

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(viewName, sustav);
            }
            else
            {
                return NotFound($"Neispravan id tima za održavanje {id}");
            }
        }

        [HttpGet]
        public IActionResult Dodaj()
        {
            ViewBag.Title = "Unos novog sustava";
            ViewBag.StudentTables = Constants.StudentTables;

            return View(new SustavViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(SustavViewModel model)
        {
            ViewBag.Title = "Dodaj sustav";
            ViewBag.StudentTables = Constants.StudentTables;
            if (ModelState.IsValid)
            {
                Sustav s = new Sustav();
                s.Opis = model.Opis;
                s.Osjetljivost = model.Osjetljivost;
                s.IdKriticnost = model.IdStupanjKriticnosti;
                s.IdVrstaSustava = model.IdVrstaSustava;
                foreach (var podsustav in model.Podsustavi)
                {
                    Podsustav noviPodsustav = new Podsustav();
                    noviPodsustav.Naziv = podsustav.Naziv;
                    noviPodsustav.Opis = podsustav.Opis;
                    noviPodsustav.Osjetljivost = podsustav.Osjetljivost;
                    noviPodsustav.UcestalostOdrzavanja = podsustav.UcestalostOdrzavanja;
                    noviPodsustav.IdKriticnost = podsustav.IdStupanjKriticnosti;
                    noviPodsustav.IdLokacija = podsustav.IdLokacija;
                    s.Podsustav.Add(noviPodsustav);
                } 

                try
                {
                    ctx.Add(s);
                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Sustav uspješno dodan. Id={s.Id}";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Uspješno dodan Sustav. Id={id}", s.Id);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    logger.LogError("Pogreška prilikom dodavanja novog sustava: {0}", exc.CompleteExceptionMessage());
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
        public async Task<IActionResult> Uredi(SustavViewModel model, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = "Uredivanje sustava";
            ViewBag.StudentTables = Constants.StudentTables;
            ViewBag.Page = page;
            ViewBag.Sort = sort;
            ViewBag.Ascending = ascending;
            if (ModelState.IsValid)
            {
                var s = await ctx.Sustav
                            .Include(d => d.Podsustav)
                            .Where(d => d.Id == model.Id)
                            .FirstOrDefaultAsync();
                if (s == null)
                {
                    return NotFound("Ne postoji sustav s id-om: " + model.Id);
                }
                s.Opis = model.Opis;
                s.Osjetljivost = model.Osjetljivost;
                s.IdKriticnost = model.IdStupanjKriticnosti;
                s.IdVrstaSustava = model.IdVrstaSustava;
                List<int> idPodsustava = model.Podsustavi
                                          .Where(r => r.Id > 0)
                                          .Select(r => r.Id)
                                          .ToList();
                ctx.RemoveRange(s.Podsustav.Where(r => !idPodsustava.Contains(r.Id)));

                foreach (var podsustav in model.Podsustavi)
                {
                    //ažuriraj postojeće i dodaj nove
                    Podsustav noviPodsustav; // potpuno nova ili dohvaćena ona koju treba izmijeniti
                    if (podsustav.Id > 0)
                    {
                        noviPodsustav = s.Podsustav.First(r => r.Id == podsustav.Id);
                    }
                    else
                    {
                        noviPodsustav = new Podsustav();
                        s.Podsustav.Add(noviPodsustav);
                    }
                    noviPodsustav.Naziv = podsustav.Naziv;
                    noviPodsustav.Opis = podsustav.Opis;
                    noviPodsustav.Osjetljivost = podsustav.Osjetljivost;
                    noviPodsustav.UcestalostOdrzavanja = podsustav.UcestalostOdrzavanja;
                    noviPodsustav.IdKriticnost = podsustav.IdStupanjKriticnosti;
                    noviPodsustav.IdLokacija = podsustav.IdLokacija;
                }
                try
                {

                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Sustav {s.Id} uspješno ažuriran.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Sustav {Id} uspješno ažuriran.", s.Id);
                    return RedirectToAction(nameof(Uredi), new
                    {
                        id = s.Id,
                        page,
                        sort,
                        ascending
                    });

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    logger.LogError("Pogreška prilikom uredivanja sustava: {0}", exc.CompleteExceptionMessage());
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
            var sustav = await ctx.Sustav
                                    .Where(t => t.Id == Id)
                                    .SingleOrDefaultAsync();
            if (sustav != null)
            {
                try
                {
                    ctx.Remove(sustav);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Sustav {sustav.Id} uspješno obrisan.";
                    TempData[Constants.ErrorOccurred] = false;
                    logger.LogInformation("Sustav {id} uspješno obrisan.", sustav.Id);
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja sustava: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja sustava: {0}", exc.CompleteExceptionMessage());
                }
            }
            else
            {
                TempData[Constants.Message] = "Ne postoji sustav s id-om: " + Id;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
        public async Task<IActionResult> Sljedeci(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = "Detalji")
        {
            ViewBag.Title = "Sustav";
            ViewBag.StudentTables = Constants.StudentTables;


            int pagesize = appSettings.PageSize;
            int pageOffset = appSettings.PageOffset;

            var query = ctx.Sustav.AsNoTracking();
            query = query.ApplySort(sort, ascending);

            var sustavi = await query
                                  .Select(s => new SustavViewModel
                                  {
                                      Id = s.Id,
                                      Opis = s.Opis,
                                      Osjetljivost = s.Osjetljivost,
                                      IdStupanjKriticnosti = s.IdKriticnostNavigation.Id,
                                      StupanjKriticnosti = s.IdKriticnostNavigation.StupanjKriticnosti,
                                      IdVrstaSustava = s.IdVrstaSustavaNavigation.Id,
                                      VrstaSustava = s.IdVrstaSustavaNavigation.NazivVrsteSustava,
                                  })
                                  .AsNoTracking()
                                  .ToListAsync();

            var stari = sustavi.Find(s => s.Id == id);
            int index = sustavi.IndexOf(stari);

            if (index != -1)
            {
                var timZaOdrzavanje = sustavi[index + 1 < sustavi.Count ? index + 1 : sustavi.Count - 1];

                return RedirectToAction(viewName, new { id = timZaOdrzavanje.Id, page, sort, ascending });
            }
            else
            {
                return NotFound($"Neispravan id sustava {id}");
            }
        }

        public async Task<IActionResult> Prethodni(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = "Detalji")
        {
            ViewBag.Title = "Sustav";
            ViewBag.StudentTables = Constants.StudentTables;


            int pagesize = appSettings.PageSize;
            int pageOffset = appSettings.PageOffset;

            var query = ctx.Sustav.AsNoTracking();
            query = query.ApplySort(sort, ascending);

            var sustavi = await query
                                  .Select(s => new SustavViewModel
                                  {
                                      Id = s.Id,
                                      Opis = s.Opis,
                                      Osjetljivost = s.Osjetljivost,
                                      IdStupanjKriticnosti = s.IdKriticnostNavigation.Id,
                                      StupanjKriticnosti = s.IdKriticnostNavigation.StupanjKriticnosti,
                                      IdVrstaSustava = s.IdVrstaSustavaNavigation.Id,
                                      VrstaSustava = s.IdVrstaSustavaNavigation.NazivVrsteSustava,
                                  })
                                  .AsNoTracking()
                                  .ToListAsync();

            var stari = sustavi.Find(s => s.Id == id);
            int index = sustavi.IndexOf(stari);

            if (index != -1)
            {
                var timZaOdrzavanje = sustavi[index - 1 < 0 ? 0 : index - 1];

                return RedirectToAction(viewName, new { id = timZaOdrzavanje.Id, page, sort, ascending });
            }
            else
            {
                return NotFound($"Neispravan id sustava {id}");
            }
        }
    }
}
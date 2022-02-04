using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using System.Threading.Tasks;
using RPPP_WebApp.Extensions.Selectors;
using System.Linq;
using System.Text.Json;
using System;
using RPPP_WebApp.Extensions;

namespace RPPP_WebApp.Controllers
{
    public class VrstaSustavaController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<VrstaSustava> logger;
        private readonly AppSettings appSettings;
        private readonly string title = "Vrste Sustava";

        public VrstaSustavaController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<VrstaSustava> logger)
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
            var query = ctx.VrstaSustava.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                logger.LogInformation("Ne postoji niti jedna vrsta sustava");
                TempData[Constants.Message] = "Ne postoji niti jedna vrsta sustava.";
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

            var vrsteSustava = await query
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            VrstaSustavaViewModel vsvm = new VrstaSustavaViewModel()
            {
                VrsteSustava = vrsteSustava,
                PagingInfo = pagingInfo
            };
            return View(vsvm);
        }

        [HttpGet]
        public async Task<IActionResult> Dodaj()
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(VrstaSustava vrstaSustava)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            logger.LogTrace(JsonSerializer.Serialize(vrstaSustava));
            if (ModelState.IsValid)
            {
                try
                {
                    await ctx.AddAsync(vrstaSustava);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation(new EventId(1000), $"Vrsta sustava uspješno dodana.");

                    TempData[Constants.Message] = $"Vrsta sustava uspješno dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanja vrste sustava: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(vrstaSustava);
                }
            }
            else
            {
                return View(vrstaSustava);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Uredi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            var vrstaSustava = await ctx.VrstaSustava
                                  .AsNoTracking()
                                  .Where(s => s.Id == id)
                                  .SingleOrDefaultAsync();
            if (vrstaSustava != null)
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(vrstaSustava);
            }
            else
            {
                return NotFound($"Neispravan unos za šifru: {id}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(VrstaSustava vrstaSustava, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = title;
            ViewBag.StudentTables = Constants.StudentTables;

            if (vrstaSustava == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.VrstaSustava.AnyAsync(v => v.Id == vrstaSustava.Id);
            if (!checkId)
            {
                return NotFound($"Neispravan status: {vrstaSustava?.Id}, {ctx.Status}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.VrstaSustava.Update(vrstaSustava);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = "Vrsta sustava ažurirana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index), new { page, sort, ascending });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());

                    return View(vrstaSustava);
                }
            }
            else
            {
                return View(vrstaSustava);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var status = await ctx.VrstaSustava.FindAsync(id);
            if (status != null)
            {
                try
                {
                    ctx.Remove(status);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Vrsta sustava sa šifrom {id} obrisana.";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = $"Pogreška prilikom brisanja kritičnosti, id: {id}: ovom vrstom sustava se koriste neki sustavi!";
                    TempData[Constants.ErrorOccurred] = true;
                }
            }
            else
            {
                TempData[Constants.Message] = $"Ne postoji stupanj vrsta sustava sa šifrom: {id}";
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
    }
}
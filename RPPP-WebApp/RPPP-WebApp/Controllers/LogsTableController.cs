using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using RPPP_WebApp.Extensions.Selectors;
using System.Linq;
using System.Threading.Tasks;

namespace RPPP_WebApp.Controllers
{
    public class LogsTableController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly ILogger<TimZaOdrzavanjeController> logger;
        private readonly AppSettings appSettings;

        public LogsTableController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<TimZaOdrzavanjeController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            this.appSettings = options.Value;
        }
        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Title = "Logovi";
            ViewBag.StudentTables = Constants.StudentTables;

            int pagesize = appSettings.PageSize;
            int pageOffset = appSettings.PageOffset;
            var query = ctx.SystemLogging.AsNoTracking();

            int count = await query.CountAsync();
            if (count == 0)
            {
                TempData[Constants.Message] = "Ne postoji niti jedan log.";
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

            var logs = await query
                          .Select(l => new LogViewModel
                          {
                              LogException = l.LogException,
                              LogMessage = l.LogMessage,
                              LogLogger = l.LogLogger,
                              LogLevel = l.LogLevel,
                              LogDate = l.LogDate,
                              EnteredDate = l.EnteredDate,
                          })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            var model = new LogsViewModel
            {
                Logs = logs,
                PagingInfo = pagingInfo
            };
            return View(model);
        }
    }
}

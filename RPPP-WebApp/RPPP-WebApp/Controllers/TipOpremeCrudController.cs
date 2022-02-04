using Microsoft.AspNetCore.Mvc;

namespace RPPP_WebApp.Controllers
{
    public class TipOpremeCrudController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Tipovi opreme";
            ViewBag.StudentTables = Constants.StudentTables;

            return View();
        }
        public IActionResult Dodaj()
        {
            ViewBag.Title = "Dodaj tip opreme";
            ViewBag.StudentTables = Constants.StudentTables;

            return View();
        }
    }
}
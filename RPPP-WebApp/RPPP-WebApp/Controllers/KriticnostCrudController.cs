using Microsoft.AspNetCore.Mvc;

namespace RPPP_WebApp.Controllers
{
    public class KriticnostCrudController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Kritičnosti";
            ViewBag.StudentTables = Constants.StudentTables;

            return View();
        }
    }
}

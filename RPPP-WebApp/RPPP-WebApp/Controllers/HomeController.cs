using Microsoft.AspNetCore.Mvc;

namespace RPPP_WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "RPPP Homework";
            ViewBag.StudentTables = Constants.StudentTables;
            return View();
        }
    }
}

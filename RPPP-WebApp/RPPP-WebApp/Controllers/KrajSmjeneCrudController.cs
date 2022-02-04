using Microsoft.AspNetCore.Mvc;

namespace RPPP_WebApp.Controllers
{
    public class KrajSmjeneCrudController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Krajevi smjena";
            ViewBag.StudentTables = Constants.StudentTables;

            return View();
        }
    }
}

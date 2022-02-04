using Microsoft.AspNetCore.Mvc;

namespace RPPP_WebApp.Controllers
{
    public class PodrucjeRadaCrudController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Podrucja radova";
            ViewBag.StudentTables = Constants.StudentTables;

            return View();
        }
    }
}

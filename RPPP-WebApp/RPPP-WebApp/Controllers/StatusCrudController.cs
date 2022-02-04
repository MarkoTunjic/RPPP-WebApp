using Microsoft.AspNetCore.Mvc;

namespace RPPP_WebApp.Controllers
{
    public class StatusCrudController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Statusi";
            ViewBag.StudentTables = Constants.StudentTables;

            return View();
        }
    }
}

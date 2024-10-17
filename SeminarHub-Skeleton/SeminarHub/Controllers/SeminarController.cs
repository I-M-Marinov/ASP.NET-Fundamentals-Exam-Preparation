using Microsoft.AspNetCore.Mvc;

namespace SeminarHub.Controllers
{
    public class SeminarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

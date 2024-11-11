using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class WineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

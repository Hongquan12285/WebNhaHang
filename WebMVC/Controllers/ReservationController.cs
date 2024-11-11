using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class ReservationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

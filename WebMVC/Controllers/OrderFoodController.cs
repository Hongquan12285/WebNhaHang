using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class OrderFoodController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class PartyMenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

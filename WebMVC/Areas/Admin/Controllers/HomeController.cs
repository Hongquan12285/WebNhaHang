using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var userTypeCookie = Request.Cookies["userRoles"];
            if (userTypeCookie != "Admin")
            {
                return RedirectToAction("Forbidden", "Error");
            }
            return View();
        }
    }
}

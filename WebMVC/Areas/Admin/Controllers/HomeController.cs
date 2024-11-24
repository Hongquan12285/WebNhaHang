using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var userNameCookie = Request.Cookies["userName"];
            var userRoleCookie = Request.Cookies["userRoles"]; // Lấy role từ cookie (ví dụ: User, Admin)

            // Gửi dữ liệu về View
            ViewData["userName"] = userNameCookie; // Tên người dùng
            ViewData["userRoles"] = userRoleCookie; // Quyền người dùng (role)


            var userTypeCookie = Request.Cookies["userRoles"];
            if (userTypeCookie != "Admin")
            {
                return RedirectToAction("Forbidden", "Error");
            }
            return View();
        }
    }
}

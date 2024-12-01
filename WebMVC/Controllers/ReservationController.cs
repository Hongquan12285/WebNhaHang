using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class ReservationController : Controller
    {
        public IActionResult Index()
        {
            var userNameCookie = Request.Cookies["userName"];
            var userRoleCookie = Request.Cookies["userRoles"]; // Lấy role từ cookie (ví dụ: User, Admin)

            // Gửi dữ liệu về View
            ViewData["userName"] = userNameCookie; // Tên người dùng
            ViewData["userRoles"] = userRoleCookie; // Quyền người dùng (role)
            return View();
        }
    }
}

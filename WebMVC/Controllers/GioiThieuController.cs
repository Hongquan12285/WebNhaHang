using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace WebMVC.Controllers
{
    public class GioiThieuController : Controller
    {
        private readonly HttpClient _httpClient;

        public GioiThieuController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

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

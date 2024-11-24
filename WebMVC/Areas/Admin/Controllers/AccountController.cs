using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebData.Models;
using System.Net.Http.Headers;
using System.Net.Http;

namespace WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, HttpClient httpClient)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _httpClient = httpClient;
        }



        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FullName = model.FullName, // Lưu FullName
                    Phone = model.Phone // Lưu Phone
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    }

                    await _userManager.AddToRoleAsync(user, "User");
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }
        // GET: Account/AddRole
        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        // POST: Account/AddRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(string role)
        {
            if (!string.IsNullOrEmpty(role))
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(role));
                    if (result.Succeeded)
                    {
                        ViewBag.Message = "Role added successfully.";
                        return View();
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Role already exists.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Role cannot be empty.");
            }

            return View();
        }

        // GET: Account/AssignRole
        [HttpGet]
        public IActionResult AssignRole()
        {
            return View();
        }

        // POST: Account/AssignRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(UserRole model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return View(model);
                }

                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    ModelState.AddModelError("", "Role does not exist.");
                    return View(model);
                }

                var result = await _userManager.AddToRoleAsync(user, model.Role);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Role assigned successfully.";
                    return View();
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        // GET: Account/Logout
        // GET: Account/Logout
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Lấy token từ cookie
                var token = Request.Cookies["Cookie"];
                if (string.IsNullOrEmpty(token))
                {
                    ModelState.AddModelError("", "Không tìm thấy token đăng nhập.");
                    return RedirectToAction("Login");
                }

                // Thêm token vào header
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Gửi yêu cầu logout tới API
                var response = await _httpClient.PostAsync("https://localhost:7228/api/Account/logout", null);

                if (response.IsSuccessStatusCode)
                {
                    // Xóa tất cả cookie liên quan
                    Response.Cookies.Delete("Cookie");
                    Response.Cookies.Delete("userId");
                    Response.Cookies.Delete("userRoles");
                    Response.Cookies.Delete("userName");

                    // Kiểm tra vai trò từ cookie
                    var userRole = Request.Cookies["userRoles"];

                    if (userRole == "Admin")
                    {
                        // Nếu vai trò là Admin, chuyển về trang login của User
                        return RedirectToAction("Login", "Account", new { area = "" });
                    }

                    // Mặc định chuyển về trang Login của Admin
                    return RedirectToAction("Login", "Account", new { area = "Admin" });
                }
                else
                {
                    // Xử lý lỗi nếu API trả về lỗi
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Đăng xuất thất bại. Chi tiết: {errorMessage}");
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                ModelState.AddModelError("", $"Đã xảy ra lỗi khi đăng xuất: {ex.Message}");
                return View("Error");
            }
        }

    }
}
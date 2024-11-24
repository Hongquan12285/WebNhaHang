using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using WebData.Models;

namespace WebMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        //// GET: Account/Register
        //[HttpGet]
        //public IActionResult Register()
        //{
        //    return View();
        //}

        //// POST: Account/Register
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Register(Register model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser
        //        {
        //            UserName = model.Username,
        //            Email = model.Email,
        //            FullName = model.FullName, // Lưu FullName
        //            Phone = model.Phone // Lưu Phone
        //        };
        //        var result = await _userManager.CreateAsync(user, model.Password);

        //        if (result.Succeeded)
        //        {
        //            if (!await _roleManager.RoleExistsAsync("User"))
        //            {
        //                await _roleManager.CreateAsync(new IdentityRole("User"));
        //            }

        //            await _userManager.AddToRoleAsync(user, "User");
        //            return RedirectToAction("Login");
        //        }

        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError("", error.Description);
        //        }
        //    }

        //    return View(model);
        //}

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7228/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PostAsJsonAsync("https://localhost:7228/api/Account/login", model);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();

                if (result != null)
                {
                    var token = result.Token;
                    var userName = result.UserName;
                    var userRoles = result.userRole;
                    var userId = result.userID;

                    Response.Cookies.Append("Cookie", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddHours(1)
                    });

                    Response.Cookies.Append("userRoles", userRoles, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddHours(1)
                    });

                    Response.Cookies.Append("userName", userName, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddHours(1)
                    });

                    Response.Cookies.Append("userId", userId, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddHours(1)
                    });

                    if (userRoles == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (userRoles == "User")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Login");
                    }
                }

                ModelState.AddModelError("", "Lỗi CMNR");
            }

            return View(model);
        }



        //// GET: Account/AddRole
        //[Authorize(Roles = "Admin")]
        //[HttpGet]
        //public IActionResult AddRole()
        //{
        //    return View();
        //}

        //// POST: Account/AddRole
        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddRole(string role)
        //{
        //    if (!string.IsNullOrEmpty(role))
        //    {
        //        if (!await _roleManager.RoleExistsAsync(role))
        //        {
        //            var result = await _roleManager.CreateAsync(new IdentityRole(role));
        //            if (result.Succeeded)
        //            {
        //                ViewBag.Message = "Role added successfully.";
        //                return View();
        //            }

        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError("", error.Description);
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Role already exists.");
        //        }
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Role cannot be empty.");
        //    }

        //    return View();
        //}

        //// GET: Account/AssignRole
        //[Authorize(Roles = "Admin")]
        //[HttpGet]
        //public IActionResult AssignRole()
        //{
        //    return View();
        //}

        //// POST: Account/AssignRole
        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AssignRole(UserRole model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByNameAsync(model.Username);
        //        if (user == null)
        //        {
        //            ModelState.AddModelError("", "User not found.");
        //            return View(model);
        //        }

        //        if (!await _roleManager.RoleExistsAsync(model.Role))
        //        {
        //            ModelState.AddModelError("", "Role does not exist.");
        //            return View(model);
        //        }

        //        var result = await _userManager.AddToRoleAsync(user, model.Role);
        //        if (result.Succeeded)
        //        {
        //            ViewBag.Message = "Role assigned successfully.";
        //            return View();
        //        }

        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError("", error.Description);
        //        }
        //    }

        //    return View(model);
        //}

        //// GET: Account/Logout
        public async Task<IActionResult> Logout()
        {
            var token = Request.Cookies["Cookie"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsync("https://localhost:7228/api/Account/logout", null);

            if (response.IsSuccessStatusCode)
            {
                // Xóa tất cả cookie liên quan
                Response.Cookies.Delete("Cookie");
                Response.Cookies.Delete("UserId");
                Response.Cookies.Delete("userRoles");
                Response.Cookies.Delete("userName");

                return RedirectToAction("Login");
            }
            else
            {
                // Xử lý lỗi nếu API trả về mã không thành công
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Đăng xuất thất bại. Chi tiết: {errorMessage}");
                return View("Error");
            }
        }

    }
}
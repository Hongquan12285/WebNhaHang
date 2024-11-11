using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebData.Models;

namespace WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        // POST: Account/AddRole
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AssignRole()
        {
            return View();
        }

        // POST: Account/AssignRole
        [Authorize(Roles = "Admin")]
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
        [Authorize]
        [HttpGet]
        public IActionResult Logout()
        {
            // Xóa cookie chứa token
            if (Request.Cookies.ContainsKey("Authorization"))
            {
                Response.Cookies.Delete("Authorization");
            }

            // Điều hướng về trang đăng nhập
            return RedirectToAction("Login", "Account", new { area = "" });
        }
    }
}
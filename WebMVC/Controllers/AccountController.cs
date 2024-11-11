using Microsoft.AspNetCore.Authentication;
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

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    // Lấy role của user
                    var userRoles = await _userManager.GetRolesAsync(user);

                    // Tạo claim cho token
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName!),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };
                    authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                    // Cấu hình token
                    var jwtSettings = _configuration.GetSection("Jwt");
                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

                    var token = new JwtSecurityToken(
                        issuer: jwtSettings["Issuer"],
                        expires: DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpiryHours"]!)),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                    var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                    // Lưu trữ token vào cookie
                    Response.Cookies.Append("Authorization", jwtToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });

                    // Điều hướng dựa trên role
                    if (userRoles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (userRoles.Contains("User"))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Login");
                    }
                }

                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
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
            return RedirectToAction("Index" , "Home");
        }
    }
}
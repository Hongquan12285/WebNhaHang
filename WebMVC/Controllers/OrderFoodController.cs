using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using WebData.Models;

namespace WebMVC.Controllers
{
    public class OrderFoodController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _client;
        private readonly ILogger<Product> _logger;

        public OrderFoodController(IHttpClientFactory httpClientFactory, HttpClient client, ILogger<Product> logger)
        {
            _httpClientFactory = httpClientFactory;
            _client = client;
            _logger = logger;
        }

        public async Task<IActionResult> Index(Product product)
        {
            var userNameCookie = Request.Cookies["userName"];
            var userRoleCookie = Request.Cookies["userRoles"]; // Lấy role từ cookie (ví dụ: User, Admin)

            // Gửi dữ liệu về View
            ViewData["userName"] = userNameCookie; // Tên người dùng
            ViewData["userRoles"] = userRoleCookie; // Quyền người dùng (role)

            var client = _httpClientFactory.CreateClient();
            var url = "https://localhost:7228/api/Product";
            var response = await client.GetAsync(url);
            var jsonstring = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<Product>>(jsonstring);
            var content = new StringContent(jsonstring, Encoding.UTF8, "application/json");
            try
            {
                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return RedirectToAction("Forbidden", "Error");
                }

                if (response.IsSuccessStatusCode)
                {
                   
                    return View(products);
                }
                else
                {
                    TempData["Error"] = "API returned an error: " + response.StatusCode;
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Đã xảy ra lỗi khi tải dữ liệu: " + ex.Message;
                return View();
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateOrder(int ProductId, string SelectedSize)
        {
            var token = Request.Cookies["Cookie"];
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var apiUrl = $"https://localhost:7228/api/Product/{ProductId}";
            var response = await httpClient.GetAsync(apiUrl);
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return RedirectToAction("Forbidden", "Error");
            }
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error retrieving product information.");
                return View();
            }
            var productJson = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<Product>(productJson);
            if (product == null || product.Quantity <= 0 || product.Price <= 0)
            {
                ModelState.AddModelError(string.Empty, "không có sản phẩm.");
                return View();
            }
            var orderDetail = new OrderDetail
            {
                ProductId = ProductId,
                Price = product.Price,
                Quantity = 1
            };
            var jsonString = JsonConvert.SerializeObject(orderDetail);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var orderApiUrl = $"https://localhost:7228/api/Order/createOrUpdate";
            var orderResponse = await httpClient.PostAsync(orderApiUrl, content);

            if (orderResponse.IsSuccessStatusCode)
            {
                var responseContent = await orderResponse.Content.ReadAsStringAsync();
                var updatedOrder = JsonConvert.DeserializeObject<Order>(responseContent);
                return RedirectToAction("ListOrder", "OrderFood");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "lỗi nhé. không cập nhập được order.");
                return View();
            }
        }

        public async Task<IActionResult> GetImage(string imgFile)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();  // Corrected this line too
                var response = await client.GetAsync($"https://localhost:7228/api/Product/GetImage/{imgFile}");

                if (response.IsSuccessStatusCode)
                {
                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                    var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";
                    return File(imageBytes, contentType);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Đã xảy ra lỗi khi tải ảnh: " + ex.Message;
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListOrder()
        {
            // Retrieve user information from cookies
            var userName = Request.Cookies["userName"];
            var token = Request.Cookies["Cookie"];
            var clientIdCookie = Request.Cookies["userId"];

            if (string.IsNullOrEmpty(clientIdCookie) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!int.TryParse(clientIdCookie, out int clientId))
            {
                ModelState.AddModelError(string.Empty, "Invalid Client ID. Please log in again.");
                return View();
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var apiUrl = $"https://localhost:7228/api/Order/user/orders";
                var response = await httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        TempData["Error"] = "Session expired. Please log in again.";
                        return RedirectToAction("Login", "Account");
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        ViewBag.Error = "No orders found for the current user.";
                        return View(new List<Order>());
                    }

                    TempData["Error"] = $"Failed to fetch orders. API returned status: {response.StatusCode}";
                    return View(new List<Order>());
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(jsonResponse);

                ViewBag.UserName = userName;
                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving orders: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while retrieving your orders.");
                return View(new List<Order>());
            }
        }

    }
}

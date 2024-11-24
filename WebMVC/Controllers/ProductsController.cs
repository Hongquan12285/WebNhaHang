using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using WebData.Models;

namespace WebMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> GetImage(string imgFile)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
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

        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                string apiUrl = $"https://localhost:7228/api/Product/GetByCategory/{categoryId}";

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<Product>>(jsonData, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return View("CategoryProducts", products);
                }

                TempData["Error"] = $"Không thể lấy dữ liệu từ API. Mã lỗi: {response.StatusCode}";
                return View("CategoryProducts", new List<Product>());
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Lỗi kết nối: {ex.Message}";
                return View("CategoryProducts", new List<Product>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Đã xảy ra lỗi không mong muốn: {ex.Message}";
                return View("CategoryProducts", new List<Product>());
            }
        }
    }
}

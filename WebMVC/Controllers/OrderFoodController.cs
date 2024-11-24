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
        private readonly IHttpClientFactory _client;
        private readonly ILogger<Product> _logger;

        public OrderFoodController(IHttpClientFactory client, ILogger<Product> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<IActionResult> Index(Product product)
        {
            var client = _client.CreateClient();
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


        public async Task<IActionResult> GetImage(string imgFile)
        {
            try
            {
                var client = _client.CreateClient();  // Corrected this line too
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
    }
}

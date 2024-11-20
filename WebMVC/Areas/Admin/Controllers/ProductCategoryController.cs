using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebData.Models;

namespace WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductCategoryController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductCategoryController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("https://localhost:7228/api/ProductCategory");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var pro = JsonConvert.DeserializeObject<List<ProductCategory>>(jsonString);
                return View(pro);
            }
            return View("Error"); 
        }


        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductCategory productCategory)
        {
            var content = new StringContent(JsonConvert.SerializeObject(productCategory), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:7228/api/ProductCategory", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View(response);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var prod = await _httpClient.GetFromJsonAsync<ProductCategory>($"https://localhost:7228/api/ProductCategory/{id}");
            return View(prod);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id , ProductCategory productCategory)
        {
            var content = new StringContent(JsonConvert.SerializeObject(productCategory), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"https://localhost:7228/api/ProductCategory/{id}", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View(response);
        }
        public async Task<IActionResult> Details(int id)
        {
            var prod = await _httpClient.GetFromJsonAsync<ProductCategory>($"https://localhost:7228/api/ProductCategory/{id}");
            return View(prod);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7228/api/ProductCategory/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View(response);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NuGet.Common;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebData.Models;

namespace WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductController(IHttpClientFactory httpClientFactory , HttpClient httpClient)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7228/api/");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IActionResult> GetImage (string imgFile)
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
        public async Task<IActionResult> Index()
        {
            
            var response = await _httpClient.GetAsync("Product");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(data);
                return View(products);
            }
            return View(new List<Product>());
        }

        public async Task<IActionResult> Create()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7228/api/ProductCategory");
            if (response.IsSuccessStatusCode)
            {
                var jsonstring = await response.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<ProductCategory>>(jsonstring);
                ViewBag.ProductCategorys = new SelectList(categories, "ID", "CategoryName"); // Ensure "ID" and "CategoryName" match your model properties
            }
            return View();
        }



        // Xử lý tạo mới sản phẩm
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(model.Title), "Title");
            formData.Add(new StringContent(model.ProductCode), "ProductCode");
            formData.Add(new StringContent(model.Description ?? ""), "Description");
            formData.Add(new StringContent(model.Detail ?? ""), "Detail");
            formData.Add(new StringContent(model.Price.ToString()), "Price");
            formData.Add(new StringContent(model.PriceSale.ToString()), "PriceSale");
            formData.Add(new StringContent(model.Quantity.ToString()), "Quantity");
            formData.Add(new StringContent(model.IsHome.ToString()), "IsHome");
            formData.Add(new StringContent(model.ProductCategoryId.ToString()), "ProductCategoryId");

            if (model.ImageFile != null)
            {
                var imageContent = new StreamContent(model.ImageFile.OpenReadStream());
                imageContent.Headers.ContentType = new MediaTypeHeaderValue(model.ImageFile.ContentType);
                formData.Add(imageContent, "ImageFile", model.ImageFile.FileName);
            }

            var response = await _httpClient.PostAsync("Product", formData);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to create product.");
            return View(model);
        }

        // Giao diện chỉnh sửa sản phẩm
        public async Task<IActionResult> Edit(int id )
        {
            var response = await _httpClient.GetAsync($"Product/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var data = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<ProductUpdateModel>(data);

            var categoryResponse = await _httpClient.GetAsync("ProductCategory");
            if (categoryResponse.IsSuccessStatusCode)
            {
                var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<ProductCategory>>(categoryData);
                ViewBag.ProductCategorys = new SelectList(categories, "ID", "CategoryName");
            }

            return View(product);
        }

        // Xử lý chỉnh sửa sản phẩm
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductUpdateModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(model.Title), "Title");
            formData.Add(new StringContent(model.ProductCode ?? ""), "ProductCode");
            formData.Add(new StringContent(model.Description ?? ""), "Description");
            formData.Add(new StringContent(model.Detail ?? ""), "Detail");
            formData.Add(new StringContent(model.Price.ToString()), "Price");
            formData.Add(new StringContent(model.PriceSale?.ToString() ?? "0"), "PriceSale");
            formData.Add(new StringContent(model.Quantity.ToString()), "Quantity");
            formData.Add(new StringContent(model.IsHome.ToString()), "IsHome");
            formData.Add(new StringContent(model.ProductCategoryId?.ToString() ?? "0"), "ProductCategoryId");

            if (model.ImageFile != null)
            {
                var imageContent = new StreamContent(model.ImageFile.OpenReadStream());
                imageContent.Headers.ContentType = new MediaTypeHeaderValue(model.ImageFile.ContentType);
                formData.Add(imageContent, "ImageFile", model.ImageFile.FileName);
            }

            var response = await _httpClient.PutAsync($"Product/{id}", formData);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to edit product.");
            return View(model);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7228/api/Product/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to delete product.");
            return RedirectToAction(nameof(Index));
        }
    }
}

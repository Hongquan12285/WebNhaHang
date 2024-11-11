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
            return View();
        }
    }
}

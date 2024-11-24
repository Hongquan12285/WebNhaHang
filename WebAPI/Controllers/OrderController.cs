using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebData.Data;
using WebData.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<IActionResult> Create([FromBody] Order order)
        //{

        //}
    }
}

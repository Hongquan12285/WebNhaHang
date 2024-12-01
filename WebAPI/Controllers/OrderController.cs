using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using WebData.Data;
using WebData.Models;

namespace WebAPI.Controllers
{
    [Authorize(Policy = "UserPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _db.Orders.ToListAsync();
            return Ok(orders);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetOrderDetailById(int id)
        {
            var orderDetail = await _db.orderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound("Order detail not found.");
            }
            return Ok(orderDetail);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var order = await _db.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(JsonConvert.SerializeObject(order, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }

        [HttpPost("createOrUpdate")]
        public async Task<IActionResult> CreateOrUpdateOrder([FromBody] OrderDetail newOrderDetail)
        {
            var userId = Request.Cookies["userId"];
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized("Cannot authenticate user. Please log in.");
            }

            var product = await _db.products.FirstOrDefaultAsync(p => p.Id == newOrderDetail.ProductId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            if (newOrderDetail == null || newOrderDetail.Quantity <= 0 || newOrderDetail.Price <= 0)
            {
                return BadRequest("Invalid product details.");
            }

            if (product.Quantity < newOrderDetail.Quantity)
            {
                return BadRequest("Not enough product quantity.");
            }

            try
            {
                var existingOrder = await _db.Orders.Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.ClientId == user.Id);

                if (existingOrder == null)
                {
                    var newOrder = new Order
                    {
                        OrderName = Request.Cookies["userName"],
                        Total = newOrderDetail.Price * newOrderDetail.Quantity,
                        Date = DateTime.Now,
                        ClientId = user.Id,
                        OrderDetails = new List<OrderDetail>()
                    };
                    newOrder.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = newOrderDetail.ProductId,
                        Quantity = newOrderDetail.Quantity,
                        Price = newOrderDetail.Price
                    });
                    product.Quantity -= newOrderDetail.Quantity;
                    _db.Orders.Add(newOrder);
                    await _db.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.Id }, newOrder);
                }
                else
                {
                    var existingOrderDetail = existingOrder.OrderDetails
                        .FirstOrDefault(od => od.ProductId == newOrderDetail.ProductId);

                    if (existingOrderDetail != null)
                    {
                        if (product.Quantity < newOrderDetail.Quantity)
                        {
                            return BadRequest("Not enough product quantity.");
                        }

                        existingOrderDetail.Quantity += newOrderDetail.Quantity;
                        existingOrderDetail.Price = newOrderDetail.Price;
                        product.Quantity -= newOrderDetail.Quantity;
                    }
                    else
                    {
                        if (product.Quantity < newOrderDetail.Quantity)
                        {
                            return BadRequest($"Product {product.Title} not available in sufficient quantity.");
                        }

                        existingOrder.OrderDetails.Add(new OrderDetail
                        {
                            ProductId = newOrderDetail.ProductId,
                            Quantity = newOrderDetail.Quantity,
                            Price = newOrderDetail.Price
                        });
                        product.Quantity -= newOrderDetail.Quantity;
                    }

                    existingOrder.Total += newOrderDetail.Price * newOrderDetail.Quantity;
                    await _db.SaveChangesAsync();
                    return Ok(existingOrder);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user/orders")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetOrdersByUser()
        {
            var userId = Request.Cookies["userId"];
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Cannot authenticate user. Please log in.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized("Cannot authenticate user. Please log in.");
            }

            var orders = await _db.Orders
                                  .Where(o => o.ClientId == user.Id)
                                  .Include(o => o.OrderDetails)
                                  .ThenInclude(od => od.product)
                                  .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound(new { message = "No orders found for this user." });
            }

            return Ok(orders);
        }

    }
}

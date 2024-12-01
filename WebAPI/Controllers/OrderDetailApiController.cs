using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebData.Data;
using WebData.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderDetailApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public OrderDetailApiController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> Delete(int id)
        {
            var orderDetail = await _db.orderDetails.Include(od => od.Order).Include(od => od.product).FirstOrDefaultAsync(od => od.ID == id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            if (orderDetail.Order != null)
            {
                orderDetail.Order.Total -= orderDetail.Price * orderDetail.Quantity;
            }
            if (orderDetail.product != null)
            {
                orderDetail.product.Quantity += orderDetail.Quantity;
            }
            _db.orderDetails.Remove(orderDetail);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                Message = "OrderDetail deleted successfully",
                UpdatedOrder = orderDetail.Order,
                UpdatedProduct = orderDetail.product
            });
        }
        [HttpPut("{id}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<ActionResult> UpdateQuantity(int id, [FromBody] OrderdetailUpdate request)
        {
            var existingDetail = await _db.orderDetails
                .Include(od => od.Order)
                .Include(od => od.product)
                .FirstOrDefaultAsync(od => od.ID == id);

            if (existingDetail == null)
            {
                return NotFound(new { Message = "OrderDetail không tồn tại!" });
            }

            if (request.NewQuantity <= 0)
            {
                return BadRequest(new { Message = "Số lượng phải lớn hơn 0!" });
            }

            var quantityDifference = request.NewQuantity - existingDetail.Quantity;

            if (existingDetail.Order != null)
            {
                existingDetail.Order.Total += quantityDifference * existingDetail.Price;
            }
            if (existingDetail.product != null)
            {
                existingDetail.product.Quantity -= quantityDifference;
                if (existingDetail.product.Quantity < 0)
                {
                    return BadRequest(new { Message = "Số lượng sản phẩm không đủ trong kho!" });
                }
            }
            existingDetail.Quantity = request.NewQuantity;
            await _db.SaveChangesAsync();

            return Ok("Cập nhật số lượng thành công!");
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetail>> GetOrderDetailById(int id)
        {
            var orderDetail = await _db.orderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            return orderDetail;
        }
    }
}

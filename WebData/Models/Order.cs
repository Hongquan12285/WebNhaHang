using System.ComponentModel.DataAnnotations;

namespace WebData.Models
{
    public class Order
    {
        public int ID { get; set; }
        [Required]
        public string Code { get; set; }
        [Required(ErrorMessage = "Tên khách hàng không để trống")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Số điện thoại không để trống")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Địa chỉ khổng để trống")]
        public string Address { get; set; }
        public string Email { get; set; }
        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public int Quantity { get; set; }
        public int TypePayment { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public string Status { get; set; }
        public string CustomerId { get; set; }
        public ICollection<OrderDetail>? orderDetails { get; set; }
    }
}

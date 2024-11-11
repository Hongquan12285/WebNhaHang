using System.ComponentModel.DataAnnotations;

namespace WebData.Models
{
    public class OrderDetail
    {
        public int ID { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public Order? Order { get; set; }
        public Product? product { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace WebData.Models
{
    public class ProductImage
    {
        public int ID { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public int? ProductId { get; set; }
        public Product? product { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebData.Models
{
    public class ProductImage
    {
        public int ID { get; set; }
        [Required]
        public string? ImageUrl { get; set; }
        public int? ProductId { get; set; }
        public Product? product { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace WebData.Models
{
    public class ProductCreateModel
    {
        [Required]
        [StringLength(250)]
        public string Title { get; set; }

        [StringLength(250)]
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }

        // Nhận file hình ảnh từ client
        public IFormFile ImageFile { get; set; }

        public decimal Price { get; set; }
        public decimal? PriceSale { get; set; }
        public int Quantity { get; set; }
        public bool IsHome { get; set; }

        // Khóa ngoại
        public int? ProductCategoryId { get; set; }
    }
}

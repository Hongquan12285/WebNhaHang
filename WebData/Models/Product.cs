using System.ComponentModel.DataAnnotations;

namespace WebData.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Title { get; set; }

        [StringLength(250)]
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }

        [StringLength(250)]
        public string Image { get; set; }
        public decimal Price { get; set; }
        public decimal? PriceSale { get; set; }
        public int Quantity { get; set; }
        public bool IsHome { get; set; }
        public int ProductCategoryId { get; set; }
        public ProductCategory? productCategory { get; set; }
        public ICollection<OrderDetail>? orderDetails { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
        public ICollection<ReviewProduct> Reviews { get; set; }
    }
}

﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public decimal? PriceSale { get; set; }
        public int Quantity { get; set; }
        public bool IsHome { get; set; }

        // Thêm khóa ngoại ProductCategoryId
        public int? ProductCategoryId { get; set; }
        [ForeignKey("ProductCategoryId")]
        public ProductCategory? ProductCategory { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }
        public ICollection<ProductImage>? ProductImages { get; set; }
        public ICollection<ReviewProduct>? Reviews { get; set; }
    }
}

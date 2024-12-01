using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebData.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string OrderName { get; set; }

        public decimal Total { get; set; }
        public DateTime Date { get; set; }

        [Required]
        public string ClientId { get; set; }
        [ForeignKey("ClientId")]
        public ApplicationUser Client { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

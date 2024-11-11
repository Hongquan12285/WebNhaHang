using System.ComponentModel.DataAnnotations;

namespace WebData.Models
{
    public class ProductCategory
    {
        public int ID { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public string Description { get; set; }


        public ICollection<Product>? products { get; set; }
    }
}

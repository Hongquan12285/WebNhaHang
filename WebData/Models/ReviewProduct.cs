using System.ComponentModel.DataAnnotations.Schema;

namespace WebData.Models
{
    public class ReviewProduct
    {
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int? ProductId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public int Rate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Avatar { get; set; }

        public virtual Product? Product { get; set; }
    }
}

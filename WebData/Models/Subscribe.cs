using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebData.Models
{
    public class Subscribe
    {
        [Key]
        public int Id { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

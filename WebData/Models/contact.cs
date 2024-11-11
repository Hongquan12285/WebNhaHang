using System.ComponentModel.DataAnnotations;

namespace WebData.Models
{
    public class contact
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(150, ErrorMessage = "Không được vượt quá 150 ký tự")]
        public string Name { get; set; }
        [StringLength(150, ErrorMessage = "Không được vượt quá 150 ký tự")]
        public string Email { get; set; }
        [Required]
        public string Subject { get; set; }

        [StringLength(4000)]
        public string Message { get; set; }
    }
}

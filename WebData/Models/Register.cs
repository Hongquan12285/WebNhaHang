using System.ComponentModel.DataAnnotations;

namespace WebData.Models
{
    public class Register
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; }
        [Required]
        [StringLength(10 , ErrorMessage = "Số điện thoại 10 số")]
        public string Phone { get; set; }
        [Required]
        public string Address { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

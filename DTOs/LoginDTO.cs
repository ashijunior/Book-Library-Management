using System.ComponentModel.DataAnnotations;

namespace BookManagement.DTOs
{
    public class LoginDTO
    {
        [Key]
        [Required(ErrorMessage = "User Name is required!")]
        [StringLength(100)]
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace BookManagement.DTOs
{
    public class RegisterDTO
    {
        [Key]
        [Required(ErrorMessage = "User Name is required!")]
        [StringLength(100)]
        public string Username { get; set; }
        [EmailAddress(ErrorMessage = "Email is required!, please input correct Email.")]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}

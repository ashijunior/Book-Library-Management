using System.ComponentModel.DataAnnotations;

namespace BookManagement.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User"; //Set to 'User' by default
        public string Token { get; set; }
    }
}

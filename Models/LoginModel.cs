using System.ComponentModel.DataAnnotations;

namespace CalorieTrackerAPI.Models
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public LoginModel(string username, string password)
        {
            Email = username;
            Password = password;
        }
    }
}

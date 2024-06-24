using System.ComponentModel.DataAnnotations;

namespace OssecAssignment.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}

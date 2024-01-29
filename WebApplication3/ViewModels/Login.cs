using System.ComponentModel.DataAnnotations;

namespace WebApplication3.ViewModels
{
    public class Login
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+[a-zA-Z]{2,4}$", ErrorMessage = "Invalid Email or Password")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
		public string Password { get; set; }
        public bool RememberMe { get; set; }
		public bool LockoutEnabled { get; set; }
		public DateTimeOffset? LockoutEnd { get; set; }

	}
}

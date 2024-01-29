using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace WebApplication3.ViewModels
{
	public class Register
	{
		[Required]
		[EmailAddress]
		[RegularExpression("^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+[a-zA-Z]{2,4}$", ErrorMessage = "Invalid Characters for Email")]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{12,}$", ErrorMessage = "Password must contain at least 12 characters, one uppercase letter, one lowercase letter, one digit, and one special character.")]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Compare(nameof(Password), ErrorMessage = "Password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		[Required]
		[RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Invalid characters for first name.")]
		public string FirstName { get; set; }

		[Required]
		[RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Invalid characters for last name.")]
		public string LastName { get; set; }

		[Required]
		public string Gender { get; set; }

		[Required]
		[RegularExpression("^[STFG]\\d{7}[A-Z]$", ErrorMessage = "Invalid NRIC format.")]
		public string NRIC { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public DateTime DateOfBirth { get; set; }

        [DataType(DataType.Upload)]
		public string? ResumeFile { get; set; }

		[RegularExpression("^[\\s\\S]*$", ErrorMessage = "Special characters are allowed.")]
		public string WhoAmI { get; set; }

		[DataType(DataType.Password)]
		public string? PasswordHistory {  get; set; }

	}
}

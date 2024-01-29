using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication3.Model
{
	public class ApplicationUser : IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Gender { get; set; }
		public string NRIC { get; set; }
		public override string Email { get; set; }
		public DateTime DateOfBirth { get; set; }
        public string? ResumeFile { get; set; }
        public string WhoAmI { get; set; }
        public DateTime? LastPasswordChanged { get; set; }
		public string? PasswordHistory { get; set; }

    }
}

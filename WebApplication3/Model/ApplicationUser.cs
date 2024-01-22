﻿using Microsoft.AspNetCore.Identity;
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
		public string Resume { get; set; }
		public string WhoAmI { get; set; }
	}
}
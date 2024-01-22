using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.ViewModels;
using WebApplication3.Model;
using System.Linq;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting.Server;
using System.IO;
using System.Threading.Tasks;

namespace WebApplication3.Pages
{

    public class RegisterModel : PageModel
	{

		private UserManager<ApplicationUser> userManager { get; }
		private SignInManager<ApplicationUser> signInManager { get; }
        private  IWebHostEnvironment environment;

        [BindProperty]
		public Register RModel { get; set; }


        public RegisterModel(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IWebHostEnvironment environment)
        {
			this.userManager = userManager;
			this.signInManager = signInManager;
            this.environment = environment;

        }


		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				/*file upload */
                /*if (RModel.Resume != null && RModel.Resume.Length > 0)
                {
                    // Validate file type and size if needed
                    var allowedExtensions = new[] { ".docx", ".pdf" };
                    var fileExtension = Path.GetExtension(RModel.Resume.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension) || RModel.Resume.Length > 102400)
                    {
                        ModelState.AddModelError("RModel.Resume", "Invalid file type or size.");
                        return Page();
                    }
                    var uploadsPath = Path.Combine(environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }

                    var fileName = Guid.NewGuid().ToString() + "_" + RModel.Resume.FileName;
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await RModel.Resume.CopyToAsync(stream);
                    }

					// Now you can save the 'fileName' to your database or use it as needed
					*//*RModel.Resume = fileName;*//*
				}*/

                var dataProtectionProvider = Microsoft.AspNetCore.DataProtection.DataProtectionProvider.Create("EncryptData");
				var protector = dataProtectionProvider.CreateProtector("MySecretKey");

				// Check if the email is unique
				var isEmailUnique = await IsEmailUniqueAsync(RModel.Email);
				if (!isEmailUnique)
				{
					ModelState.AddModelError(nameof(RModel.Email), "Email address is already in use.");
					return Page();
				}


				var user = new ApplicationUser()
				{
					FirstName = RModel.FirstName,
					LastName = RModel.LastName,
					Email = RModel.Email,
					Gender = RModel.Gender,
					NRIC = protector.Protect(RModel.NRIC),
					DateOfBirth = RModel.DateOfBirth,
					Resume = RModel.Resume,
					WhoAmI = RModel.WhoAmI,
				};

				var result = await userManager.CreateAsync(user, RModel.Password);
				if (result.Succeeded)
				{
					await signInManager.SignInAsync(user, false);
					return RedirectToPage("Login");
				}

				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
		
	return Page();
	}




        async Task<bool> IsEmailUniqueAsync(string Email)
			{

				var existingUser = await userManager.FindByEmailAsync(Email);
				return existingUser == null;
			}



		}
	}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.DataProtection;
using WebApplication3.Model;
using WebApplication3.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace WebApplication3.Pages
{
    [Authorize] 
    public class IndexModel : PageModel
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDataProtectionProvider dataProtectionProvider;
        private readonly ILogger<IndexModel> logger;

        [BindProperty]
		public IndexPage IModel { get; set; }


        public IndexModel(UserManager<ApplicationUser> userManager, IDataProtectionProvider dataProtectionProvider, ILogger<IndexModel> logger)
        {
            this.userManager = userManager;
            this.dataProtectionProvider = dataProtectionProvider;
            this.logger = logger;
        }

		public IndexPage UserInfo { get; private set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var dataProtectionProvider = Microsoft.AspNetCore.DataProtection.DataProtectionProvider.Create("EncryptData");
            var protector = dataProtectionProvider.CreateProtector("MySecretKey");

            var user = await userManager.GetUserAsync(User);

            if (user != null)
            {
                IModel = new IndexPage
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Gender = user.Gender,
                    NRIC = protector.Unprotect(user.NRIC),
                    DateOfBirth = user.DateOfBirth,
                    WhoAmI = user.WhoAmI
                };

                return Page();
            }

            return Page();
        }

    }
	}
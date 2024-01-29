using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.DataProtection;
using WebApplication3.Model;
using WebApplication3.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;

namespace WebApplication3.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDataProtectionProvider dataProtectionProvider;
        private readonly ILogger<IndexModel> logger;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly int maxPasswordAgeMinutes;

        public string PasswordChangeNotification { get; private set; }

        [BindProperty]
        public IndexPage UserInfo { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager, IDataProtectionProvider dataProtectionProvider, ILogger<IndexModel> logger, SignInManager<ApplicationUser> signInManager, int maxPasswordAgeMinutes = 3)
        {
            this.userManager = userManager;
            this.dataProtectionProvider = dataProtectionProvider;
            this.logger = logger;
            this.signInManager = signInManager;
            this.maxPasswordAgeMinutes = maxPasswordAgeMinutes;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            await CheckSessionAndAuthenticationAsync();

            var dataProtectionProvider = Microsoft.AspNetCore.DataProtection.DataProtectionProvider.Create("EncryptData");
            var protector = dataProtectionProvider.CreateProtector("MySecretKey");

            try
            {
                UserInfo = new IndexPage
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Gender = user.Gender,
                    NRIC = protector.Unprotect(user.NRIC),
                    DateOfBirth = user.DateOfBirth,
                    WhoAmI = user.WhoAmI
                };

                CheckPasswordAge(user);

                return Page();
            }
            catch (Exception ex)
            {
                // Log the exception details for troubleshooting
                logger.LogError(ex, "Error decrypting data");
                throw; // Re-throw the exception to avoid hiding the error
            }
        }

        private async Task CheckSessionAndAuthenticationAsync()
        {
            if (HttpContext.Session.GetString("AuthToken") == null || !HttpContext.Session.GetString("AuthToken").Equals(Request.Cookies["AuthToken"]))
            {
                await signInManager.SignOutAsync();
                HttpContext.Session.Clear();

                // Remove cookies
                Response.Cookies.Delete("ASP.NET_SessionId");
                Response.Cookies.Delete("AuthToken");
                RedirectToPage("Login");
            }
        }

        private void CheckPasswordAge(ApplicationUser user)
        {
            if (user.LastPasswordChanged != null)
            {
                var maxAgeElapsed = DateTime.UtcNow - user.LastPasswordChanged.Value;
                var roundedMinutes = Math.Round(maxAgeElapsed.TotalMinutes);
                if (roundedMinutes >= maxPasswordAgeMinutes)
                {
                    PasswordChangeNotification = $"You must change your password as it has been {roundedMinutes} minutes since the last change.";
                }
            }
        }

    }
}

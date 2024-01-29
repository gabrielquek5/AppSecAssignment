using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.ViewModels;
using WebApplication3.Model;
using System.Linq;
using Microsoft.AspNetCore.DataProtection;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Net;
using System.Text.Encodings.Web;

namespace WebApplication3.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment environment;

        [BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            this.userManager = userManager;
            this.environment = environment;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (await ValidateCaptchaAsync())
                {
                    var dataProtectionProvider = Microsoft.AspNetCore.DataProtection.DataProtectionProvider.Create("EncryptData");
                    var protector = dataProtectionProvider.CreateProtector("MySecretKey");

                    // Check if the email is unique
                    var isEmailUnique = await IsEmailUniqueAsync(RModel.Email);
                    if (!isEmailUnique)
                    {
                        ModelState.AddModelError(nameof(RModel.Email), "Email address is already in use.");
                        return Page();
                    }

                    // Hash the initial password
                    var initialPasswordHash = userManager.PasswordHasher.HashPassword(null, RModel.Password);

                    var user = new ApplicationUser()
                    {
                        FirstName = RModel.FirstName,
                        LastName = RModel.LastName,
                        Email = RModel.Email,
                        Gender = RModel.Gender,
                        NRIC = protector.Protect(RModel.NRIC),
                        DateOfBirth = RModel.DateOfBirth,
                        ResumeFile = RModel.ResumeFile,
                        WhoAmI = HtmlEncoder.Default.Encode(RModel.WhoAmI),
                        UserName = RModel.Email,
                        // Store the hashed initial password in the PasswordHistory
                        PasswordHistory = initialPasswordHash
                    };

                    var result = await userManager.CreateAsync(user, RModel.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToPage("Login");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    ModelState.AddModelError("", "Captcha validation failed");
                }
            }

            return Page();
        }

        async Task<bool> IsEmailUniqueAsync(string Email)
        {
            var existingUser = await userManager.FindByEmailAsync(Email);
            return existingUser == null;
        }

        private async Task<bool> ValidateCaptchaAsync()
        {
            bool result = true;

            // When user submits the recaptcha form, the user gets a response POST parameter.
            // captchaResponse consist of the user click pattern. Behaviour analytics! AI :)
            string captchaResponse = Request.Form["g-recaptcha-response"];

            // To send a GET request to Google along with the response and Secret key.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LcunE4pAAAAAEPbqG7YKaqKHIDfluXku5EgYT6g&response=" + captchaResponse);

            try
            {
                // Codes to receive the Response in JSON format from Google Server
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        // The response in JSON format
                        string jsonResponse = readStream.ReadToEnd();

                        // Deserialize JSON response
                        var jsonObject = JsonSerializer.Deserialize<Login>(jsonResponse);

                        // Create jsonObject to handle the response e.g success or Error
                    }
                }

                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
    }
}

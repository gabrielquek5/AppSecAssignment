using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using WebApplication3.Model;
using System.Threading.Tasks;

namespace WebApplication3.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<ApplicationUser> signInManager;

        public PrivacyModel(ILogger<PrivacyModel> logger, IHttpContextAccessor httpContextAccessor, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            this.signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> OnGet()
        {
            // Check if the AuthToken session variable exists and matches the value in the cookie
            if (HttpContext.Session.GetString("AuthToken") == null || !HttpContext.Session.GetString("AuthToken").Equals(Request.Cookies["AuthToken"]))
            {
                await signInManager.SignOutAsync();
                HttpContext.Session.Clear();

                // Remove cookies
                Response.Cookies.Delete("ASP.NET_SessionId");
                Response.Cookies.Delete("AuthToken");
                return RedirectToPage("Login");
            }

            // If the session is authenticated, return the privacy page
            return Page();
        }
    }
}

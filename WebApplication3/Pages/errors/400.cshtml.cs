using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;

namespace WebApplication3.Pages.errors
{
    public class _400Model : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;

        public _400Model(SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> OnGet()
        {
            await signInManager.SignOutAsync();
            HttpContext.Session.Clear();

            // Remove cookies
            Response.Cookies.Delete("ASP.NET_SessionId");
            Response.Cookies.Delete("AuthToken");
            return RedirectToPage("/Login");
        }
    }
}

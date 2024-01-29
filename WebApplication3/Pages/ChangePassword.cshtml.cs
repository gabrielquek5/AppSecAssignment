using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;
using WebApplication3.ViewModels;

namespace WebApplication3.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly AuthDbContext dbContext;

        public ChangePasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AuthDbContext dbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.dbContext = dbContext;
        }

        [BindProperty]
        public ChangePassword CPModel { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if the user is authenticated
            if (!signInManager.IsSignedIn(User))
            {
                // Redirect to the login page if the session is timed out
                return RedirectToPage("/Login");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if the user is authenticated
            if (!signInManager.IsSignedIn(User))
            {
                // Redirect to the login page if the session is timed out
                return RedirectToPage("/Login");
            }

            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
                }

                var isCurrentPasswordValid = await userManager.CheckPasswordAsync(user, CPModel.OldPassword);
                if (!isCurrentPasswordValid)
                {
                    ModelState.AddModelError(nameof(CPModel.OldPassword), "The current password is incorrect.");
                    return Page();
                }
                var isSamePassword = await userManager.CheckPasswordAsync(user, CPModel.NewPassword);
                if (isSamePassword)
                {
                    ModelState.AddModelError(nameof(CPModel.NewPassword), "New password cannot be the same as the current password.");
                    return Page();
                }

                // Hash the new password before checking password history
                var newPasswordHash = userManager.PasswordHasher.HashPassword(user, CPModel.NewPassword);
                var passwordHistory = user.PasswordHistory?.Split(',');

                // Check password history
                if (passwordHistory != null && passwordHistory.Any(p => p == newPasswordHash))
                {
                    ModelState.AddModelError(string.Empty, "Password has been used before. Please choose a different one.");
                    return Page();
                }

                const int maxPasswordHistoryCount = 2; // Example limit
                var newPasswordHistory = string.Join(',', new[] { newPasswordHash }.Concat(passwordHistory).Take(maxPasswordHistoryCount));

                var changePasswordResult = await userManager.ChangePasswordAsync(user, CPModel.OldPassword, CPModel.NewPassword);
                if (changePasswordResult.Succeeded)
                {
                    // Update password history
                    user.PasswordHistory = newPasswordHistory;
                    user.LastPasswordChanged = DateTime.UtcNow;
                    await userManager.UpdateAsync(user);
                    await signInManager.RefreshSignInAsync(user);
                    TempData["SuccessMessage"] = "Password changed successfully!";
                    return RedirectToPage("Index");
                }

                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay the form
            return Page();
        }
    }
}

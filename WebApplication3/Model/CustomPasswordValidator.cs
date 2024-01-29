using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Model;

namespace WebApplication3.Model
{
    public class CustomPasswordValidator : IPasswordValidator<ApplicationUser>
    {
        private const int MaxPasswordHistoryCount = 2;
        private readonly int minPasswordAgeMinutes;
        private readonly int maxPasswordAgeMinutes;

        public CustomPasswordValidator(int minPasswordAgeMinutes = 1, int maxPasswordAgeMinutes = 3)
        {
            this.minPasswordAgeMinutes = minPasswordAgeMinutes;
            this.maxPasswordAgeMinutes = maxPasswordAgeMinutes;
        }

        public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string password)
        {
            var errors = new List<IdentityError>();

            // Check minimum password age
            if (user.LastPasswordChanged != null)
            {
                var minAgeElapsed = DateTime.UtcNow - user.LastPasswordChanged.Value;
                if (minAgeElapsed.TotalMinutes < minPasswordAgeMinutes)
                {
                    errors.Add(new IdentityError
                    {
                        Code = "PasswordChangeTooSoon",
                        Description = $"You cannot change your password again within the next {minPasswordAgeMinutes} minutes."
                    });
                }
            }

            // Check maximum password age
            /*if (user.LastPasswordChanged != null)
            {
                var maxAgeElapsed = DateTime.UtcNow - user.LastPasswordChanged.Value;
                if (maxAgeElapsed.TotalMinutes >= maxPasswordAgeMinutes)
                {
                    errors.Add(new IdentityError
                    {
                        Code = "PasswordChangeRequired",
                        Description = $"You must change your password as it has been {maxAgeElapsed.TotalMinutes} minutes since the last change."
                    });
                }
            }*/

            // Fetch the user's password history
            var passwordHistory = user.PasswordHistory?.Split(',');

            // Check if the new password matches any of the previous passwords
            if (passwordHistory != null && passwordHistory.Contains(password))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordReuse",
                    Description = "Password has been used before. Please choose a different one."
                });
            }

            // Remove oldest password if history count exceeds the limit
            if (passwordHistory?.Length >= MaxPasswordHistoryCount)
            {
                passwordHistory = passwordHistory.Skip(1).ToArray(); // Remove the oldest password
                user.PasswordHistory = string.Join(',', passwordHistory);
            }

            return Task.FromResult(errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}

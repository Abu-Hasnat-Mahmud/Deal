using Deal.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deal.Repository
{
    public interface IAccountRepository
    {
        Task<IdentityResult> SignUpUserAsync(SignUpUserModel userModel);
        Task<SignInResult> PasswordSignInAsync(SignInUserModel userModel);
        Task Signout();
        Task<IdentityResult> ChangePasswordAsync(ChangePasswordModel model);
        Task<IdentityResult> ConfirmEmail(string uId, string token);

        Task GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task GenerateEmailForgotPasswordTokenAsync(ApplicationUser user);

        Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model);
    }
}

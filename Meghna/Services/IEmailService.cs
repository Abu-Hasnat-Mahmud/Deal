using Deal.Models;
using System.Threading.Tasks;

namespace Deal.Services
{
    public interface IEmailService
    {
        Task SendTestEmail(UserEmailOptions userEmailOptions);
        Task SendEmailConfirmation(UserEmailOptions userEmailOptions);
        Task SendEmailForgotPassword(UserEmailOptions userEmailOptions);
        Task SendEmailCheckoutClient(UserEmailOptions userEmailOptions);
        Task SendEmailCheckoutSeller(UserEmailOptions userEmailOptions); 
        Task SendEmailLinkClient(UserEmailOptions userEmailOptions);
        Task SendEmailLinkAuthor(UserEmailOptions userEmailOptions);

        Task SendEmailQRClient(UserEmailOptions userEmailOptions);
        Task SendEmailQRAuthor(UserEmailOptions userEmailOptions);


    }
}   
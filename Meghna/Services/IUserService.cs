using Deal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deal.Services
{
    public interface IUserService
    {
        string GetUserId();
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<List<ApplicationUser>> GetUsersAsync();
        bool IsAuthenticated();
    }
}
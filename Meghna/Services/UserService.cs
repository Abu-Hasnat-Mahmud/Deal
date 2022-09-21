using Deal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Deal.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(IHttpContextAccessor httpContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _httpContext = httpContext;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public string GetUserId()
        {
            return _httpContext.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
        public async Task<List<ApplicationUser>> GetUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public bool IsAuthenticated()
        {
            return _httpContext.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}

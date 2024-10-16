
using Core.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Extentions
{
    public static class UserMangerExtension
    {
        public static async Task<AppUser> FindUserByClaimsPrincipleWithAddress(this UserManager<AppUser> userManager, ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            return await userManager.Users.Include(x => x.Address).SingleOrDefaultAsync(x => x.Email == email);
        }

        public static async Task<AppUser> FindByEmailFromclaimPrinciple(this UserManager<AppUser> userManager, ClaimsPrincipal User)
        {
            return await userManager.Users.SingleOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
        }

    }
}


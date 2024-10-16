using Core.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "CarHubTeam",
                    Email = "CarHubITI@gmail.com",
                    UserName = "CarHubITI@gmail.com",
                    Address =new Address
                    {
                        FirstName="Car",
                        LastName="Hub",
                        Street="Elglaa st",
                        City="Mansoura",
                        State="EG",
                        ZipCode="123"

                    }
                };
                await userManager.CreateAsync(user,"P@ssw0rd");
                
            }
        }
    }
}

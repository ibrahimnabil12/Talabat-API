using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> _userManager)
        {
            if (_userManager.Users.Count()==0) 
            {
                var user = new AppUser()
                {
                    DisplayName = "ibrahim nabil",
                    Email = "slorks1@gmail.com",
                    UserName = "ibrahimNail",
                    PhoneNumber = "01010823511"
                };
               await _userManager.CreateAsync(user,"Pa$$W0rd"); // we pass password here as createAsync hashes it 
            }
        }
      
    }
}

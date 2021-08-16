using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.DataAccesLayer
{
    public class DataInitiailizer
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataInitiailizer(AppDbContext dbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeeDataAsync()
        {
            await _dbContext.Database.MigrateAsync();

            #region Role seed

            var roles = new List<string>
            {
                RoleConstants.Admin,
                RoleConstants.Member
            };

            foreach (var role in roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                    continue;

                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            #endregion

            #region User seed

            User dbUser = _dbContext.Users.FirstOrDefault(x => x.Email == "admin@code.az");
            if (dbUser == null)
            {
                var user = new User
                {
                    Email = "admin@code.az",
                    UserName = "Admin",
                    Name = "Admin",
                    Surname = "Admin"
                };

                if (await _userManager.FindByEmailAsync(user.Email) == null)
                {
                    await _userManager.CreateAsync(user, "Admin@123");
                    await _userManager.AddToRoleAsync(user, RoleConstants.Admin);
                }
            }
            #endregion
        }
    }
}

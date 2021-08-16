using FinalProject.DataAccesLayer;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = RoleConstants.Admin)]
    public class UserController : Controller
    {
        private readonly AppDbContext _dbContext;
        public readonly UserManager<User> _userManager;
        public readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var dbUsers = await _userManager.Users.ToListAsync();
            var users = new List<UserViewModel>();

            foreach (var dbUser in dbUsers)
            {
                var user = new UserViewModel
                {
                    Id = dbUser.Id,
                    Username = dbUser.UserName,
                    Email = dbUser.Email,
                    Name = dbUser.Name,
                    Surname = dbUser.Surname,
                    Role = (await _userManager.GetRolesAsync(dbUser)).FirstOrDefault(),
                    IsDeactive = dbUser.IsDeleted
                };
                users.Add(user);
            }
            return View(users);
        }

        #region ChangePassword
        public async Task<IActionResult> ChangePassword(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var userViewModel = new ChangePasswordViewModel
            {
                Username = user.UserName
            };

            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string id, ChangePasswordViewModel changePassword)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            if (!ModelState.IsValid)
            {
                return View();
            }

            var dbUser = await _userManager.FindByIdAsync(id);
            if (dbUser == null)
            {
                return BadRequest();
            }

            var changePasswordViewModel = new ChangePasswordViewModel
            {
                Username = dbUser.UserName
            };

            if (!await _userManager.CheckPasswordAsync(dbUser, changePassword.OldPassword))
            {
                ModelState.AddModelError("OldPassword", "Old password is not valid");
                return View(changePasswordViewModel);
            }

            var result = await _userManager.ChangePasswordAsync(dbUser, changePassword.OldPassword, changePassword.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(changePasswordViewModel);
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region ChangeRole
        public async Task<IActionResult> ChangeRole(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            ChangeRoleViewModel changeRole = new ChangeRoleViewModel
            {
                Username = user.UserName,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                Roles = new List<string>()
            };
            changeRole.Roles.Add(RoleConstants.Admin);
            changeRole.Roles.Add(RoleConstants.Member);

            return View(changeRole);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string id, string selectedRole, List<int?> CoursesId)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.Users.Where(x => x.IsDeleted == false).FirstOrDefaultAsync(y => y.Id == id);

            if (user == null)
                return NotFound();

            ChangeRoleViewModel changeRole = new ChangeRoleViewModel
            {
                Username = user.UserName,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                Roles = new List<string>()
            };
            changeRole.Roles.Add(RoleConstants.Admin);
            changeRole.Roles.Add(RoleConstants.Member);

            if (selectedRole == null)
            {
                ModelState.AddModelError("", "Bosh ola bilmez");
                return View(changeRole);
            }

            var addResult = await _userManager.AddToRoleAsync(user, selectedRole);
            if (!addResult.Succeeded)
            {
                foreach (var error in addResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(changeRole);
            }
            var removeResult = await _userManager.RemoveFromRoleAsync(user, changeRole.Role);
            if (!removeResult.Succeeded)
            {
                foreach (var error in removeResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(changeRole);
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Activity
        public async Task<IActionResult> Activity(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (user.IsDeleted == false)
            {
                user.IsDeleted = true;
            }
            else
            {
                user.IsDeleted = false;
            }

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index");
        }

        #endregion

    }
}

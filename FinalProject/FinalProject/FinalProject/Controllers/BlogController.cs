using FinalProject.DataAccesLayer;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;


        public BlogController(AppDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            List<Blog> blogs = await _dbContext.Blogs.Where(x => x.IsDelete == false).Include(x => x.Comments.Where(x => x.IsDelete == false)).ToListAsync();
            return View(blogs);
        }

        #endregion

        #region Detail
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return NotFound();

            TempData["BlogId"] = id;

            CommentViewModel commentViewModel = new CommentViewModel
            {
                BlogDetail = await _dbContext.BlogDetails.Where(x => x.IsDelete == false).Include(x => x.Blog).OrderByDescending(x => x.Id).FirstOrDefaultAsync(x => x.BlogId == id),
                Comments = _dbContext.Comments.Where(c => c.BlogId == id && c.IsDelete == false).ToList(),
            };

            if (commentViewModel.BlogDetail == null)
                return NotFound();

            return View(commentViewModel);
        }

        #endregion

        #region BlogComment
        [HttpPost]
        public async Task<IActionResult> BlogComment(string name, string email, string message)
        {
            int id = (int)TempData["BlogId"];

            if (message == null)
                return NotFound();

            Comment comment = new Comment();
            if (User.Identity.IsAuthenticated)
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                comment.Name = user.Name;
                comment.Email = user.Email;
            }
            else
            {
                comment.Name = "Guest-" + name;
                comment.Email = email;
            }

            comment.Message = message;
            comment.CreateTime = DateTime.UtcNow;
            comment.BlogId = id;

            if (comment == null)
                return NotFound();

            await _dbContext.Comments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();
            return PartialView("_CommentsPartial", comment);

        }
        #endregion

    }
}

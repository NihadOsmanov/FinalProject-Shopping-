using FinalProject.DataAccesLayer;
using FinalProject.Models;
using Layihe.Areas.AdminPanel.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = RoleConstants.Admin)]
    public class BlogController : Controller
    {
        private readonly AppDbContext _dbContext;

        public BlogController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index(int page = 1)
        {
            ViewBag.PageCount = Math.Ceiling((decimal)_dbContext.Blogs.Where(s => s.IsDelete == false).Count() / 4);
            ViewBag.Page = page;

            if (ViewBag.PageCount < page || page <= 0)
            {
                return NotFound();
            }

            var blogs = _dbContext.Blogs.Where(x => x.IsDelete == false).Include(x => x.BlogDetail).OrderByDescending(x => x.Id)
                                                                                            .Skip(((int)page - 1) * 4).Take(4).ToList();
            return View(blogs);
        }

        #region Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (blog.Photo == null)
            {
                ModelState.AddModelError("Photo", "Please select Photo");
                return View();
            }

            if (!blog.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Not the image you uploaded");
                return View();
            }

            if (!blog.Photo.IsSizeAllowed(1024))
            {
                ModelState.AddModelError("Photo", "The size of the uploaded image is high");
                return View();
            }

            if(blog.Date > DateTime.Now)
            {
                ModelState.AddModelError("Date", "The date time of the now date time is high");
                return View();
            }

            var isExist = await _dbContext.Blogs.Where(x => x.IsDelete == false).AnyAsync(x => x.Title.ToLower() == blog.Title.ToLower());
            if (isExist)
            {
                ModelState.AddModelError("Title", "There is a blog with this title");
                return View();
            }

            var fileName = await FileUtil.GenerateFileAsync(Constants.BlogImageFolderPath, blog.Photo);

            blog.Image = fileName;
            blog.IsDelete = false;

            await _dbContext.Blogs.AddAsync(blog);
            blog.BlogDetail.BlogId = blog.Id;
            await _dbContext.BlogDetails.AddAsync(blog.BlogDetail);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Update
        public IActionResult Update(int? id)
        {
            if (id == null)
                return NotFound();

            var dbBlog = _dbContext.Blogs.Where(x => x.IsDelete == false).Include(x => x.BlogDetail).FirstOrDefault(y => y.Id == id);

            if (dbBlog == null)
                return NotFound();

            return View(dbBlog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Blog blog)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            var dbBlog = _dbContext.Blogs.Where(x => x.IsDelete == false).Include(x => x.BlogDetail).FirstOrDefault(y => y.Id == id);

            if (dbBlog == null)
                return NotFound();

            if (blog.Photo != null)
            {
                if (!blog.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Not the image you uploaded");
                    return View();
                }

                if (!blog.Photo.IsSizeAllowed(1024))
                {
                    ModelState.AddModelError("Photo", "The size of the uploaded image is high");
                    return View();
                }

                var path = Path.Combine(Constants.BlogImageFolderPath, dbBlog.Image);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                var fileName = await FileUtil.GenerateFileAsync(Constants.BlogImageFolderPath, blog.Photo);
                dbBlog.Image = fileName;
            }

            dbBlog.Posted = blog.Posted;
            dbBlog.Title = blog.Title;
            dbBlog.Date = blog.Date;
            dbBlog.BlogDetail.Description = blog.BlogDetail.Description;

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Detail
        public IActionResult Detail(int? id)
        {
            if (id == null)
                return NotFound();

            var blogDetail = _dbContext.BlogDetails.Where(x => x.IsDelete == false).Include(x => x.Blog).FirstOrDefault(y => y.BlogId == id);

            if (blogDetail == null)
                return NotFound();

            return View(blogDetail);
        }

        #endregion

        #region Delete
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var blogDetail = _dbContext.BlogDetails.Where(x => x.IsDelete == false).Include(x => x.Blog).FirstOrDefault(y => y.BlogId == id);

            if (blogDetail == null)
                return NotFound();

            return View(blogDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteCourse(int? id)
        {
            if (id == null)
                return NotFound();

            var blogDetail = _dbContext.BlogDetails.Where(x => x.IsDelete == false).Include(x => x.Blog).FirstOrDefault(y => y.BlogId == id);

            if (blogDetail == null)
                return NotFound();

            blogDetail.IsDelete = true;
            blogDetail.Blog.IsDelete = true;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    #endregion
    }
}

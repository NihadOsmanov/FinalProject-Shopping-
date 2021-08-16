using FinalProject.DataAccesLayer;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
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
    public class CategoryController : Controller
    {
        private readonly AppDbContext _dbContext;

        public CategoryController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var categories = _dbContext.Categories.ToList();

            return View(categories);
        }
        #region Delete
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var categories = _dbContext.Categories.ToList();

            if (categories == null)
                return NotFound();

            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null)
                return NotFound();

            var categories = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (categories == null)
                return NotFound();

            _dbContext.Categories.Remove(categories);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var isExist = _dbContext.Categories.Any(x => x.Name.ToLower() == category.Name.ToLower());
            if (isExist)
            {
                ModelState.AddModelError("Name", "Bu adda kategoriya var");
                return View();
            }

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (id == null)
                return NotFound();

            var dbCategory = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (dbCategory == null)
                return NotFound();

            var isExist = await _dbContext.Categories.AnyAsync(x => x.Name.ToLower() == category.Name.ToLower() && x.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "Bu adda kategoriya var");
                return View();
            }

            dbCategory.Name = category.Name;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        #endregion
    }
}

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
    public class SizeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public SizeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var sizes = _dbContext.Sizes.ToList();
            return View(sizes);
        }

        #region Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Size size)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var isExist = _dbContext.Sizes.Any(x => x.Name.ToLower() == size.Name.ToLower());
            if (isExist)
            {
                ModelState.AddModelError("Name", "Bu adda size var");
                return View();
            }

            await _dbContext.Sizes.AddAsync(size);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var size = await _dbContext.Sizes.FirstOrDefaultAsync(x => x.Id == id);

            if (size == null)
                return NotFound();

            return View(size);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Size size)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (id == null)
                return NotFound();

            var dbSize = await _dbContext.Sizes.FirstOrDefaultAsync(x => x.Id == id);

            if (dbSize == null)
                return NotFound();

            var isExist = await _dbContext.Sizes.AnyAsync(x => x.Name.ToLower() == size.Name.ToLower() && x.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "Bu adda size var");
                return View();
            }

            dbSize.Name = size.Name;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var size = await _dbContext.Sizes.FirstOrDefaultAsync(x => x.Id == id);

            if (size == null)
                return NotFound();

            return View(size);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteSize(int? id)
        {

            if (id == null)
                return NotFound();

            var size = await _dbContext.Sizes.FindAsync(id);

            if (size == null)
                return NotFound();

            _dbContext.Sizes.Remove(size);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        #endregion
    }
}

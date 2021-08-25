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
    public class BrandController : Controller
    {
        private readonly AppDbContext _dbContext;

        public BrandController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var brand = _dbContext.Brands.Where(x => x.IsDeleted == false).ToList();
            return View(brand);
        }

        #region Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var brand = await _dbContext.Brands.Where(x => x.IsDeleted == false).FirstOrDefaultAsync(x => x.Id == id);

            if (brand == null)
                return NotFound();

            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteBrand(int? id)
        {
            if (id == null)
                return NotFound();

            var brands = await _dbContext.Brands.Where(x => x.IsDeleted == false).FirstOrDefaultAsync(x => x.Id == id);

            if (brands == null)
                return NotFound();

            _dbContext.Brands.Remove(brands);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var isExist = _dbContext.Brands.Where(x => x.IsDeleted == false).Any(x => x.Name.ToLower() == brand.Name.ToLower());
            if (isExist)
            {
                ModelState.AddModelError("Name", "Bu adda brand var");
                return View();
            }

            await _dbContext.Brands.AddAsync(brand);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var brand = await _dbContext.Brands.Where(x => x.IsDeleted == false).FirstOrDefaultAsync(x => x.Id == id);

            if (brand == null)
                return NotFound();

            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (id == null)
                return NotFound();

            var dbBrand = await _dbContext.Brands.Where(x => x.IsDeleted == false).FirstOrDefaultAsync(x => x.Id == id);

            if (dbBrand == null)
                return NotFound();

            var isExist = await _dbContext.Brands.Where(x => x.IsDeleted == false).AnyAsync(x => x.Name.ToLower() == brand.Name.ToLower() && x.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "Bu adda brand var");
                return View();
            }

            dbBrand.Name = brand.Name;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        #endregion
    }
}

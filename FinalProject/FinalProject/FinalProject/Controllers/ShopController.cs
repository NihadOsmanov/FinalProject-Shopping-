using FinalProject.DataAccesLayer;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ShopController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Index
        public async Task<IActionResult> Index()

        {
            List<Product> products = await _dbContext.Products.Where(x => x.IsDelete == false).OrderByDescending(x => x.Id).ToListAsync();
            Shop shop = await _dbContext.Shops.Where(x => x.IsDelete == false).FirstOrDefaultAsync();
            List<Brand> brands = await _dbContext.Brands.ToListAsync();

            ShopViewModel shopViewModel = new ShopViewModel()
            {
                Products = products,
                Shop = shop,
                Brands = brands
            };
            return View(shopViewModel);
        }

        #endregion

        #region Detail
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return NotFound();

            ProductDetail productDetail = await _dbContext.ProductDetails.Include(x => x.Product).ThenInclude(x => x.ProductSizes).ThenInclude
                                                                                        (x => x.Size).FirstOrDefaultAsync(x => x.ProductId == id);

            if (productDetail == null)
                return NotFound();


            Shop shop = await _dbContext.Shops.Where(x => x.IsDelete == false).FirstOrDefaultAsync();

            ShopDetailViewModel shopDetailViewModel = new ShopDetailViewModel()
            {
                ProductDetail = productDetail,
                Shop = shop
            };
            return View(shopDetailViewModel);

        }

        #endregion

        #region Search
        public IActionResult Search(string search)
        {
            if (search == null)
                return NotFound();

            var products = _dbContext.Products.Where(x => x.IsDelete == false && x.Name.Contains(search)).ToList();

            return PartialView("_ShopViewPartial", products);
        }

        public async Task<IActionResult> Filtering(string productId)
        {
            return Json(productId);
        }

        #endregion
    }
}

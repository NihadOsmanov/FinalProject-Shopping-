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
            List<Size> sizes = await _dbContext.Sizes.ToListAsync();

            ShopViewModel shopViewModel = new ShopViewModel()
            {
                Products = products,
                Shop = shop,
                Brands = brands,
                Sizes = sizes
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


        #endregion

        #region Filter
        public async Task<IActionResult> Filtering(List<string> productIds, List<string> sizeIds,string price)
        {
            List<Product> products = new List<Product>();
            if (productIds.Count != 0 || sizeIds.Count != 0)
            {
                if(productIds.Count != 0 && sizeIds.Count != 0)
                {
                    foreach (var productId in productIds)
                    {
                        foreach (var sizeId in sizeIds)
                        {
                            products = await _dbContext.Products.Include(x => x.ProductSizes).ThenInclude(x => x.Size)
                                .Where(x => x.BrandId == Convert.ToInt32(productId) && x.ProductSizes.Any(x => x.SizeId == Convert.ToInt32(sizeId)))
                                .OrderByDescending(x => x.Id).ToListAsync();
                        }
                    }
                }
                else if(productIds.Count != 0)
                {
                    foreach (var productId in productIds)
                    {
                        products = await _dbContext.Products.Where(x => x.BrandId == Convert.ToInt32(productId)).OrderByDescending(x => x.Id).ToListAsync();
                    }
                }
                else
                {
                    foreach (var productId in sizeIds)
                    {
                        products = await _dbContext.Products.Where(x => x.BrandId == Convert.ToInt32(productId)).OrderByDescending(x => x.Id).ToListAsync();
                    }
                }
            }
            else
            {
                products = await _dbContext.Products.OrderByDescending(x => x.Id).ToListAsync();
            }

            if (!string.IsNullOrEmpty(price))
            {
                var product = products.Where(x => Convert.ToDouble(x.Price) - ((Convert.ToDouble(x.Price) * x.Discount / 100)) <= Convert.ToDouble(price)).ToList();
                return PartialView("_ShopViewPartial", product);
            }

            return PartialView("_ShopViewPartial", products);
        }

        #endregion
    }
}

using FinalProject.DataAccesLayer;
using FinalProject.Models;
using FinalProject.Utils;
using Layihe.Areas.AdminPanel.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    public class ShopController : Controller
    {
        private readonly AppDbContext _dbContext;
        public readonly IWebHostEnvironment _enviroment;

        public ShopController(AppDbContext dbContext, IWebHostEnvironment enviroment)
        {
            _dbContext = dbContext;
            _enviroment = enviroment;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.PageCount = Math.Ceiling((decimal)_dbContext.Products.Where(s => s.IsDelete == false).Count() / 4);
            ViewBag.Page = page;

            if (ViewBag.PageCount < page || page <= 0)
            {
                return NotFound();
            }
            List<Product> products = await _dbContext.Products.Where(x => x.IsDelete == false).Include(x => x.ProductDetail).OrderByDescending(x => x.Id)
                                                        .Skip(((int)page - 1) * 4).Take(4).ToListAsync();

            return View(products);
        }
        #region Create
        public IActionResult Create()
        {
            ViewBag.Categories = _dbContext.Categories.ToList();
            ViewBag.Sizes = _dbContext.Sizes.ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, int? CategoryId, List<int?> SizesId)
        {
            var categories = _dbContext.Categories.ToList();
            ViewBag.Categories = categories;
            var sizes = _dbContext.Sizes.ToList();
            ViewBag.Sizes = sizes;

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (CategoryId == null)
            {
                ModelState.AddModelError("", "Please select Category");
                return View();
            }

            if (categories.All(x => x.Id != CategoryId.Value))
            {
                return BadRequest();
            }

            if (SizesId.Count == 0)
            {
                ModelState.AddModelError("", "Please select Sizes");
                return View();
            }

            foreach (var size in SizesId)
            {
                if (sizes.All(x => x.Id != (int)size))
                {
                    return BadRequest();
                }
            }

            if (product.Photo == null)
            {
                ModelState.AddModelError("Photo", "Please select Photo");
                return View();
            }

            if (!product.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Not the image you uploaded");
                return View();
            }

            if (!product.Photo.IsSizeAllowed(2048))
            {
                ModelState.AddModelError("Photo", "The size of the uploaded image is high");
                return View();
            }

            var fileName = await FileUtil.GenerateFileAsync(Constants.ProductImageFolderPath, product.Photo);

            product.Image = fileName;
            product.IsDelete = false;

            var productSizes = new List<ProductSize>();

            foreach (var ps in SizesId)
            {
                var productSize = new ProductSize();
                productSize.ProductId = product.Id;
                productSize.SizeId = (int)ps;
                productSizes.Add(productSize);
            }

            if (Convert.ToDouble(product.NewPice) < 0)
            {
                ModelState.AddModelError("", "Price can not be less than 0");
                return View();
            }

            if (Convert.ToDouble(product.OldPrice) <= Convert.ToDouble(product.NewPice))
            {
                ModelState.AddModelError("", "Oldprice can not be less than NewPrice");
                return View();
            }

            if (Convert.ToDouble(product.OldPrice) < 0)
            {
                ModelState.AddModelError("", "Price can not be less than 0");
                return View();
            }

            if (product.Rate < 0)
            {
                ModelState.AddModelError("", "Rate can not be less than 0");
                return View();
            }

            product.CategoryId = CategoryId.Value;
            product.ProductSizes = productSizes;
            await _dbContext.AddRangeAsync(product, product.ProductDetail);
            await _dbContext.SaveChangesAsync();

            string href = "https://localhost:44325/Shop/Detail/" + product.Id;
            string subject = "New Product Added";
            string msgBody = $"<a href={href}>New Product Add for see you click here</a> ";

            foreach (var item in (await _dbContext.Subscribers.ToListAsync()))
            {
                await Helper.SendMessage(subject, msgBody, item.Email);
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Detail
        public IActionResult Detail(int? id)
        {
            if (id == null)
                return NotFound();

            var product = _dbContext.Products.Where(x => x.IsDelete == false).Include(x => x.ProductDetail).Include(x => x.Category)
                                        .Include(x => x.ProductSizes).ThenInclude(x => x.Size).FirstOrDefault(y => y.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        #endregion

        #region Update
        public IActionResult Update(int? id)
        {
            var categories = _dbContext.Categories.ToList();
            ViewBag.Categories = categories;
            var sizes = _dbContext.Sizes.ToList();
            ViewBag.Sizes = sizes;

            if (id == null)
                return NotFound();

            var product = _dbContext.Products.Where(x => x.IsDelete == false).Include(x => x.ProductDetail).Include(x => x.ProductSizes).FirstOrDefault(y => y.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Product product, int? CategoryId, List<int?> SizesId)
        {
            
            var categories = _dbContext.Categories.ToList();
            ViewBag.Categories = categories;
            var sizes = _dbContext.Sizes.ToList();
            ViewBag.Sizes = sizes;

            if (!ModelState.IsValid)
            {
                return View();
            }

            var dbProduct = _dbContext.Products.Where(x => x.IsDelete == false).Include(x => x.ProductDetail).Include(x => x.ProductSizes)
                                                        .ThenInclude(x => x.Size).FirstOrDefault(y => y.Id == id);

            if (dbProduct == null)
                return NotFound();

            if (CategoryId == null)
            {
                ModelState.AddModelError("", "Please select Category");
                return View();
            }

            if (SizesId == null)
            {
                ModelState.AddModelError("", "Please select Size");
                return View();
            }

            if (categories.All(x => x.Id != CategoryId))
            {
                return BadRequest();
            }

            foreach (var size in SizesId)
            {
                if (sizes.All(x => x.Id != (int)size))
                {
                    return BadRequest();
                }
            }

            if (product.Photo != null)
            {
                if (!product.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Not the image you uploaded");
                    return View();
                }

                if (!product.Photo.IsSizeAllowed(1024))
                {
                    ModelState.AddModelError("Photo", "The size of the uploaded image is high");
                    return View();
                }

                var path = Path.Combine(Constants.ProductImageFolderPath, dbProduct.Image);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                var fileName = await FileUtil.GenerateFileAsync(Constants.ProductImageFolderPath, product.Photo);
                dbProduct.Image = fileName;
            }

            var productSizes = new List<ProductSize>();

            foreach (var ps in SizesId)
            {
                var productSize = new ProductSize();
                productSize.ProductId = product.Id;
                productSize.SizeId = (int)ps;
                productSizes.Add(productSize);
            }

            if (Convert.ToInt32(product.NewPice) < 0)
            {
                ModelState.AddModelError("", "Price can not be less than 0");
                return View();
            }

            if (Convert.ToInt32(product.OldPrice) < 0)
            {
                ModelState.AddModelError("", "Price can not be less than 0");
                return View();
            }

            dbProduct.Name = product.Name;
            dbProduct.ProductDetail.Detail = product.ProductDetail.Detail;
            dbProduct.OldPrice = product.OldPrice;
            dbProduct.NewPice = product.NewPice;
            dbProduct.Rate = product.Rate;
            dbProduct.ProductDetail.Febric = product.ProductDetail.Febric;
            dbProduct.ProductDetail.Color = product.ProductDetail.Color;
            dbProduct.ProductDetail.Material = product.ProductDetail.Material;
            dbProduct.CategoryId = (int)CategoryId;
            dbProduct.ProductSizes = productSizes;

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Delete
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = _dbContext.Products.Where(x => x.IsDelete == false).Include(x => x.ProductDetail).OrderByDescending(t => t.Id)
                                                                                                        .FirstOrDefault(y => y.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteCourse(int? id)
        {
            if (id == null)
                return NotFound();

            var product = _dbContext.Products.Where(x => x.IsDelete == false).Include(x => x.ProductDetail).OrderByDescending(t => t.Id)
                                                                                                      .FirstOrDefault(y => y.Id == id);

            if (product == null)
                return NotFound();

            product.IsDelete = true;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion
    }
}

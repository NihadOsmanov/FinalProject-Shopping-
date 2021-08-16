using FinalProject.DataAccesLayer;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public HomeController(AppDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            Slider slider = await _dbContext.Sliders.FirstOrDefaultAsync();
            List<Product> products = await _dbContext.Products.Where(x => x.IsDelete == false).Include(x => x.Category).ToListAsync();
            List<Category> categories = await _dbContext.Categories.ToListAsync();
            Parallax parallax = await _dbContext.Parallaxes.FirstOrDefaultAsync();
            List<Space> spaces = await _dbContext.Spaces.Where(x => x.IsDelete == false).ToListAsync();


            HomeViewModel homeViewModel = new HomeViewModel()
            {
                Slider = slider,
                Products = products,
                Categories = categories,
                Parallax = parallax,
                Spaces = spaces
            };
            return View(homeViewModel);
        }

        #endregion

        #region AddToBasket
        public async Task<IActionResult> AddToBasket(int? id)
        {
            if (id == null)
                return NotFound();

            Product product = await _dbContext.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            List<BasketViewModel> productsList;

            var basketCookie = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(basketCookie))
            {
                productsList = new List<BasketViewModel>();
            }
            else
            {
                productsList = JsonConvert.DeserializeObject<List<BasketViewModel>>(basketCookie);
            }

            var existProduct = productsList.FirstOrDefault(x => x.Id == id);
            if (existProduct == null)
            {
                productsList.Add(new BasketViewModel { Id = product.Id });
            }
            else
            {
                existProduct.Count++;
            }

            var productJson = JsonConvert.SerializeObject(productsList);
            Response.Cookies.Append("basket", productJson);
            return Json(productsList.Count);

        }

        #endregion

        #region Increase
        public async Task<IActionResult> Increase(int? id)
        {
            if (id == null)
                return NotFound();

            Product product = await _dbContext.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            List<BasketViewModel> productsList;

            var basketCookie = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(basketCookie))
            {
                productsList = new List<BasketViewModel>();
            }
            else
            {
                productsList = JsonConvert.DeserializeObject<List<BasketViewModel>>(basketCookie);
            }

            var existProduct = productsList.FirstOrDefault(x => x.Id == id);
            existProduct.Count++;

            var result = new List<BasketViewModel>();
            foreach (var basketViewModel in productsList)
            {
                var dbProduct = _dbContext.Products.Find(basketViewModel.Id);
                if (dbProduct == null)
                    continue;

                basketViewModel.Price = Convert.ToDouble(dbProduct.NewPice);
                //foreach (var size in dbProduct.ProductSizes)
                //{
                //    basketViewModel.Size = size.Size.Name;
                //}
                basketViewModel.Image = dbProduct.Image;
                basketViewModel.ProductName = dbProduct.Name;

                result.Add(basketViewModel);
            }

            var productJson = JsonConvert.SerializeObject(result);
            Response.Cookies.Append("basket", productJson);

            return PartialView("_cartBasketPartial", result);
        }

        #endregion

        #region Decrease
        public async Task<IActionResult> Decrease(int? id)
        {
            if (id == null)
                return NotFound();

            Product product = await _dbContext.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            List<BasketViewModel> productsList;

            var basketCookie = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(basketCookie))
            {
                productsList = new List<BasketViewModel>();
            }
            else
            {
                productsList = JsonConvert.DeserializeObject<List<BasketViewModel>>(basketCookie);
            }

            var existProduct = productsList.FirstOrDefault(x => x.Id == id);
            if (existProduct.Count > 1)
            {
                existProduct.Count--;
            }
            else
            {
                productsList.Remove(existProduct);
            }

            var result = new List<BasketViewModel>();
            foreach (var basketViewModel in productsList)
            {
                var dbProduct = _dbContext.Products.Find(basketViewModel.Id);
                if (dbProduct == null)
                    continue;

                basketViewModel.Price = Convert.ToDouble(dbProduct.NewPice);
                //foreach (var size in dbProduct.ProductSizes)
                //{
                //    basketViewModel.Size = size.Size.Name;
                //}
                basketViewModel.Image = dbProduct.Image;
                basketViewModel.ProductName = dbProduct.Name;

                result.Add(basketViewModel);
            }

            var productJson = JsonConvert.SerializeObject(result);
            Response.Cookies.Append("basket", productJson);

            return PartialView("_cartBasketPartial", result);
        }

        #endregion

        #region Basket
        public IActionResult Basket()
        {
            var cookieBasket = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(cookieBasket))
                return View("No data found is Basket");

            var basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(cookieBasket);
            var result = new List<BasketViewModel>();
            foreach (var basketViewModel in basketViewModels)
            {
                var dbProduct = _dbContext.Products.Find(basketViewModel.Id);
                if (dbProduct == null)
                    continue;

                basketViewModel.Price = Convert.ToDouble(dbProduct.NewPice);
                //foreach (var size in dbProduct.ProductSizes)
                //{
                //    basketViewModel.Size = size.Size.Name;
                //}
                basketViewModel.Image = dbProduct.Image;
                basketViewModel.ProductName = dbProduct.Name;

                result.Add(basketViewModel);
            }

            var basket = JsonConvert.SerializeObject(result);

            return View(result);
        }

        #endregion

        #region Delete
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = _dbContext.Products.Find(id);

            if (product == null)
                return NotFound();

            var cookieBasket = Request.Cookies["basket"];
            var basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(cookieBasket);

            var existProduct = basketViewModels.FirstOrDefault(x => x.Id == id);

            if (existProduct.Count > 1)
            {
                existProduct.Count--;
            }
            else
            {
                basketViewModels.Remove(existProduct);
            }
            var productJson = JsonConvert.SerializeObject(basketViewModels);
            Response.Cookies.Append("basket", productJson);

            return RedirectToAction("Basket");
        }

        #endregion

        #region RemoveAll
        public IActionResult RemoveAll(int? id)
        {
            if (id == null)
                return NotFound();

            var product = _dbContext.Products.Find(id);

            if (product == null)
                return NotFound();

            var cookieBasket = Request.Cookies["basket"];
            var basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(cookieBasket);

            var existProduct = basketViewModels.Find(x => x.Id == id);
            basketViewModels.Remove(existProduct);

            var productJson = JsonConvert.SerializeObject(basketViewModels);

            Response.Cookies.Append("basket", productJson);
            return RedirectToAction("Basket");
        }

        #endregion

        #region Search
        public IActionResult Search(string search)
        {
            if (search == null)
                return NotFound();

            var products = _dbContext.Products.Where(x => x.IsDelete == false && x.Name.Contains(search)).ToList();

            return PartialView("_SearchViewPartial", products);
        }

        #endregion

        #region Subscriber
        public async Task<IActionResult> Subscriber(string email)
        {
            if (!User.Identity.IsAuthenticated)
            {
                string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;.](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";

                if (email == null)
                {
                    return Content("Email can not empty");
                }

                if (!Regex.IsMatch(email, pattern))
                {
                    return Content("Email is not valid");
                }
            }
            else
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                email = user.Email;
            }
            var dbSubsriber = _dbContext.Subscribers.ToList();

            var subscriber = new Subscriber()
            {
                Email = email
            };

            foreach (var item in dbSubsriber)
            {
                if (item.Email == email)
                {
                    return Content("this account allready is subscriber");
                }
            }

            await _dbContext.AddAsync(subscriber);
            await _dbContext.SaveChangesAsync();

            return Content("Suceess");
        }

        #endregion

    }
}

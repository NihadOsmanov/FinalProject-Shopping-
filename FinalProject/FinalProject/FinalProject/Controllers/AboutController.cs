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
    public class AboutController : Controller
    {
        private readonly AppDbContext _dbContext;

        public AboutController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            About about = await _dbContext.Abouts.FirstOrDefaultAsync();
            List<Testimonial> testimonials = await _dbContext.Testimonials.Where(x => x.IsDelete == false).ToListAsync();
            List<Employee> employees = await _dbContext.Employees.Where(x => x.IsDelete == false).ToListAsync();

            AboutViewModel aboutView = new AboutViewModel()
            {
                About = about,
                Testimonials = testimonials,
                Employees = employees
            };
            return View(aboutView);
        }

        #endregion
    }
}

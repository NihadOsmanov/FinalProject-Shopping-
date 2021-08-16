using FinalProject.DataAccesLayer;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class FaqController : Controller
    {
        private readonly AppDbContext _dbContext;

        public FaqController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            List<Faq> faqs = await _dbContext.Faqs.Where(x => x.IsDelete == false).ToListAsync();
            return View(faqs);
        }
    }
}

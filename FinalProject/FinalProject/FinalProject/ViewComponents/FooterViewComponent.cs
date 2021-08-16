using FinalProject.DataAccesLayer;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly AppDbContext _dbContext;

        public FooterViewComponent(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Layout layout = await _dbContext.Layouts.FirstOrDefaultAsync();
            Payment payment = await _dbContext.Payments.FirstOrDefaultAsync();

            PayLayViewModel payLay = new PayLayViewModel()
            {
                Layout = layout,
                Payments = payment
            };
            return View(payLay);
        }
    } 
}

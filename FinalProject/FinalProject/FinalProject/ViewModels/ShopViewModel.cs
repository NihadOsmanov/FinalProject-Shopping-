using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels
{
    public class ShopViewModel
    {
        public Shop Shop { get; set; }
        public ProductDetail ProductDetail { get; set; }
        public List<Product> Products { get; set; }
        public List<Size> Sizes { get; set; }
        public List<Brand> Brands { get; set; }
    }
}

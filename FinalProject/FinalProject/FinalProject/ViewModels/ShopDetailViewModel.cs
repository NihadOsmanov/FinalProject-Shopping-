using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels
{
    public class ShopDetailViewModel
    {
        public Shop Shop { get; set; }
        public ProductDetail ProductDetail { get; set; }
        public Product Product { get; set; }
    }
}

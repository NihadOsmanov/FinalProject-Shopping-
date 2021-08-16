using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels
{
    public class HomeViewModel
    {
        public Slider Slider { get; set; }
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public Parallax Parallax { get; set; }
        public List<Space> Spaces { get; set; }
    }
}

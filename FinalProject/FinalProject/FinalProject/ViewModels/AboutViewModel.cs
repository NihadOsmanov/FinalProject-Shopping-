using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels
{
    public class AboutViewModel
    {
        public About About { get; set; }
        public List<Testimonial> Testimonials { get; set; }
        public List<Employee> Employees { get; set; }
    }
}

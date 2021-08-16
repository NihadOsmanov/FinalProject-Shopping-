using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels
{
    public class BlogViewModel
    {
        public List<Blog> Blogs { get; set; }
        public BlogDetail BlogDetail { get; set; }
    }
}

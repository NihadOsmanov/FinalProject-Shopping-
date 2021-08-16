using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels
{
    public class CommentViewModel
    {
        public List<Comment> Comments { get; set; }
        public BlogDetail BlogDetail { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class Space
    {
        public int Id { get; set; }

        [Required]
        public string Image { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }
        public bool IsDelete { get; set; }
    }
}

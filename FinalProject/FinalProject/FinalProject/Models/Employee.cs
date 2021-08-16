using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Fullname { get; set; }
        public string  Profession { get; set; }

        [Required]
        public string  Image { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }

        public bool IsDelete { get; set; }
    }
}

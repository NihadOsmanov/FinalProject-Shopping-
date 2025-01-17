﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int Rate { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        
        public string Price { get; set; }
        public int Discount { get; set; }

        public string Image { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }

        [Required]
        public bool IsDelete { get; set; }
        public ProductDetail ProductDetail { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }
        public ICollection<ProductSize> ProductSizes { get; set; }

        public int BrandId { get; set; }

        public Brand Brand { get; set; }

    }
}

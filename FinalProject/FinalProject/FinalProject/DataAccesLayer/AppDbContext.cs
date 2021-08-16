using FinalProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.DataAccesLayer
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Layout> Layouts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Parallax> Parallaxes { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Space> Spaces { get; set; }
        public DbSet<BlogDetail> BlogDetails { get; set; }
        public DbSet<About> Abouts { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<Employee> Employees{ get; set; }
        public DbSet<Faq> Faqs{ get; set; }
        public DbSet<Shop> Shops{ get; set; }
        public DbSet<Subscriber> Subscribers{ get; set; }
        public DbSet<ProductDetail> ProductDetails{ get; set; }
        public DbSet<Category> Categories{ get; set; }
        public DbSet<Comment> Comments{ get; set; }
        public DbSet<Size> Sizes{ get; set; }
        public DbSet<ProductSize> ProductSizes{ get; set; }
        public DbSet<EmailToMe> EmailToMes { get; set; }

        public DbSet<Brand> Brands { get; set; }
    }
}

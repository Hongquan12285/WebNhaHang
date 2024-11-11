using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebData.Models;

namespace WebData.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Product> products { get; set; }
        public DbSet<ProductCategory> productCategories { get; set; }
        public DbSet<ProductImage> productImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> orderDetails { get; set; }
        public DbSet<Subscribe> subscribes { get; set; }
        public DbSet<ReviewProduct> reviewProducts { get; set; }
        public DbSet<contact> contacts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-1640V9E\\SQLEXPRESS;Database=websiteNhaHang;Trusted_Connection=True;TrustServerCertificate=True");
        }
    }
}

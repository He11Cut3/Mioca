using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Miocas.Models;

namespace Miocas.Infrastructure
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<Product> Products { get; set; }
        public DbSet<ItemCart> ItemCarts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        // ... остальные настройки контекста

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ItemCart>()
                .HasOne(ci => ci.User)
                .WithMany(u => u.ItemCarts)
                .HasForeignKey(ci => ci.UserId);
        }
    }
}

using Cafe.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Infrastructure.Data
{
    public class CafeDbContext(DbContextOptions<CafeDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(e =>
            {
                e.HasKey(c => c.Id);
                e.Property(c => c.Name).IsRequired().HasMaxLength(100);
                e.HasIndex(c => c.Name).IsUnique();

                e.HasData(
                    new Category { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Cafés" },
                    new Category { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Chás" },
                    new Category { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Doces" });
            });

            modelBuilder.Entity<Product>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Name).IsRequired().HasMaxLength(150);
                e.Property(p => p.Description).HasMaxLength(500);
                e.Property(p => p.Price).HasPrecision(10, 2);
                e.Property(p => p.RowVersion).IsRowVersion();
                e.HasIndex(p => p.IsActive);

                e.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasData(
                    new Product { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Espresso", Price = 8.50m, Stock = 100, CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111") },
                    new Product { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Cappuccino", Price = 12.00m, Stock = 80, CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111") },
                    new Product { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Chá Verde", Price = 9.00m, Stock = 60, CategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222") },
                    new Product { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Brownie", Price = 10.50m, Stock = 40, CategoryId = Guid.Parse("33333333-3333-3333-3333-333333333333") });
            });

            modelBuilder.Entity<Order>(e =>
            {
                e.HasKey(o => o.Id);
                e.Property(o => o.Status).IsRequired().HasMaxLength(20);
                e.Property(o => o.Total).HasPrecision(12, 2);
                e.Property(o => o.CreatedAt);
                e.HasOne(o => o.User)
                    .WithMany()
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(o => o.UserId);
            });

            modelBuilder.Entity<OrderItem>(e =>
            {
                e.HasKey(i => i.Id);
                e.Property(i => i.UnitPrice).HasPrecision(10, 2);

                e.HasOne(i => i.Order)
                    .WithMany(o => o.Items)
                    .HasForeignKey(i => i.OrderId)
                    .OnDelete(DeleteBehavior.Cascade); // deletar pedido leva os itens

                e.HasOne(i => i.Product)
                    .WithMany()
                    .HasForeignKey(i => i.ProductId)
                    .OnDelete(DeleteBehavior.Restrict); // não deletar produto com pedido

                e.HasIndex(i => i.OrderId);
            });

            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.Id);
                e.Property(u => u.Name).IsRequired().HasMaxLength(150);
                e.Property(u => u.Email).IsRequired().HasMaxLength(255);
                e.Property(u => u.PasswordHash).IsRequired();
                e.Property(u => u.Role).IsRequired().HasMaxLength(20);
                e.HasIndex(u => u.Email).IsUnique();
            });
        }
    }
}

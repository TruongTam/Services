using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CosmeticShop.Models;
using CosmeticShop.ViewModels;

namespace CosmeticShop.Data
{
    public class CosmeticShopDbContext : DbContext
    {
        // sương
        public CosmeticShopDbContext(DbContextOptions<CosmeticShopDbContext> options) : base(options) { }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<CartStatus> CartStatuses { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentReply> CommentReplies { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<PayStatus> PayStatuses { get; set; }
        public DbSet<PayType> PayTypes { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Slug> Slugs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AnoCart> AnoCarts { get; set; }
        public DbSet<AnoCartDetail> AnoCartDetails { get; set; }

        // Tạo chỉ mục Unique Index cho các trường của từng bảng
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Bảng Role
            modelBuilder.Entity<Role>().HasIndex(r => r.Name).IsUnique();

            // Bảng Admin
            modelBuilder.Entity<Admin>().HasIndex(a => a.Username).IsUnique();
            modelBuilder.Entity<Admin>().HasIndex(a => a.Email).IsUnique();
            //  modelBuilder.Entity<Product>().HasIndex(a => a.Name).IsUnique();
            // Bảng Setting
            modelBuilder.Entity<Setting>().HasIndex(s => s.Name).IsUnique();

            // Bảng User
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.PhoneNumber).IsUnique();

            // Bảng ProductBrand
            modelBuilder.Entity<ProductBrand>().HasIndex(b => b.Name).IsUnique();

            // Bảng ProductType
            modelBuilder.Entity<ProductType>().HasIndex(t => t.Type).IsUnique();

            // Bảng Slug
            modelBuilder.Entity<Slug>().HasIndex(s => s.Url).IsUnique();

            // Bảng Product      không cần Unique Index
            // Bảng Comment      không cần Unique Index
            // Bảng CommentReply không cần Unique Index

            // Bảng CartStatus
            modelBuilder.Entity<CartStatus>().HasIndex(c => c.Status).IsUnique();

            // Bảng Cart         không cần Unique Index
            // Bảng CartDetail   không cần Unique Index

            // Bảng OrderStatus
            modelBuilder.Entity<OrderStatus>().HasIndex(o => o.Status).IsUnique();

            // Bảng PayType
            modelBuilder.Entity<PayType>().HasIndex(p => p.Type).IsUnique();

            // Bảng PayStatus
            modelBuilder.Entity<PayStatus>().HasIndex(p => p.Status).IsUnique();

            // Bảng Order         không cần Unique Index
            // Bảng OrderDetail   không cần Unique Index
        }

        // Public các ViewModel

    }
}

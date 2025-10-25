using GGOnlineShop.Models;
using Microsoft.EntityFrameworkCore;
using System;

using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace GGOnlineShop.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 只保留球星卡相关的DbSet
        public DbSet<Category> Categories { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<UserInfo> Users { get; set; }
        public DbSet<UserCard> UserCards { get; set; }
        public DbSet<CartItem> CartItems { get; set; } // 购物车项

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 应用实体配置
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new CardConfiguration());
            modelBuilder.ApplyConfiguration(new UserCardConfiguration());
            modelBuilder.ApplyConfiguration(new UserInfoConfiguration());
            

          

            // 配置CartItem的关联
            modelBuilder.Entity<CartItem>(builder =>
            {
                builder.HasKey(ci => new { ci.CardId, ci.UserId }); // 联合主键

                builder.HasOne(ci => ci.Card)
                       .WithMany()
                       .HasForeignKey(ci => ci.CardId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(ci => ci.User)
                       .WithMany(u => u.CartItems)
                       .HasForeignKey(ci => ci.UserId)
                       .OnDelete(DeleteBehavior.Cascade);
            });

            // 初始化分类数据（之前的代码保留）
            SeedCategories(modelBuilder);
        }

        private void SeedCategories(ModelBuilder modelBuilder)
        {
            // 1. 顶级分类 All
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "All", ParentId = null, SortOrder = 1, CreateTime = DateTime.Now }
            );

            // 2. 二级分类 Basketball、Football
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 2, Name = "Basketball", ParentId = 1, SortOrder = 2, CreateTime = DateTime.Now },
                new Category { Id = 3, Name = "Football", ParentId = 1, SortOrder = 3, CreateTime = DateTime.Now }
            );

            // 3. 三级分类 - Basketball 子分类
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 4, Name = "NBA", ParentId = 2, SortOrder = 4, CreateTime = DateTime.Now },
                new Category { Id = 5, Name = "FIBA", ParentId = 2, SortOrder = 5, CreateTime = DateTime.Now },
                new Category { Id = 6, Name = "CBA", ParentId = 2, SortOrder = 6, CreateTime = DateTime.Now }
            );

            // 4. 三级分类 - Football 子分类
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 7, Name = "Serie A", ParentId = 3, SortOrder = 7, CreateTime = DateTime.Now },
                new Category { Id = 8, Name = "La Liga", ParentId = 3, SortOrder = 8, CreateTime = DateTime.Now },
                new Category { Id = 9, Name = "Premier League", ParentId = 3, SortOrder = 9, CreateTime = DateTime.Now }
            );
        }
    }
}
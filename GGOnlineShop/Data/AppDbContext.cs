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

        // Only keep DbSets related to player cards
        public DbSet<Category> Categories { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<UserCard> UserCards { get; set; }
        public DbSet<CartItem> CartItems { get; set; } // Cart items
        public DbSet<SubCategory> SubCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new CardConfiguration());
            modelBuilder.ApplyConfiguration(new UserCardConfiguration());
            modelBuilder.ApplyConfiguration(new UserInfoConfiguration());
            
                
          

            // Configure relationships for CartItem
            modelBuilder.Entity<CartItem>(builder =>
            {
                builder.HasKey(ci => new { ci.CardId, ci.UserId }); // Composite primary key

                builder.HasOne(ci => ci.Card)
                       .WithMany()
                       .HasForeignKey(ci => ci.CardId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(ci => ci.User)
                       .WithMany(u => u.CartItems)
                       .HasForeignKey(ci => ci.UserId)
                       .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed category data (previous code retained)
            SeedCategories(modelBuilder);
        }

        private void SeedCategories(ModelBuilder modelBuilder)
        {
            // 1. Top-level category: All
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "All", ParentId = null, SortOrder = 1, CreateTime = DateTime.Now }
            );

            // 2. Second-level categories: Basketball, Football
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 2, Name = "Basketball", ParentId = 1, SortOrder = 2, CreateTime = DateTime.Now },
                new Category { Id = 3, Name = "Football", ParentId = 1, SortOrder = 3, CreateTime = DateTime.Now }
            );

            // 3. Third-level categories - Basketball subcategories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 4, Name = "NBA", ParentId = 2, SortOrder = 4, CreateTime = DateTime.Now },
                new Category { Id = 5, Name = "FIBA", ParentId = 2, SortOrder = 5, CreateTime = DateTime.Now },
                new Category { Id = 6, Name = "CBA", ParentId = 2, SortOrder = 6, CreateTime = DateTime.Now }
            );

            // 4. Third-level categories - Football subcategories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 7, Name = "Serie A", ParentId = 3, SortOrder = 7, CreateTime = DateTime.Now },
                new Category { Id = 8, Name = "La Liga", ParentId = 3, SortOrder = 8, CreateTime = DateTime.Now },
                new Category { Id = 9, Name = "Premier League", ParentId = 3, SortOrder = 9, CreateTime = DateTime.Now }
            );
        }
    }
}
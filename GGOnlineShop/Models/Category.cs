using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GGOnlineShop.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;

        // 导航属性
        public virtual Category ParentCategory { get; set; }
        public virtual ICollection<Category> ChildCategories { get; set; }
        public virtual ICollection<Card> Cards { get; set; }
    }

    // Fluent API 配置
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
            builder.Property(c => c.SortOrder).IsRequired();

            builder.HasOne(c => c.ParentCategory)
                   .WithMany(c => c.ChildCategories)
                   .HasForeignKey(c => c.ParentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}
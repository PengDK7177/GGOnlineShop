using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GGOnlineShop.Models
{
    public class Card
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }

        [Required] // 必填属性
        public string PlayerName { get; set; } = string.Empty;

        public string Team { get; set; } = string.Empty;
        public int CardYear { get; set; }
        public string CardGrade { get; set; } = string.Empty;
        public decimal CardPrice { get; set; }
        public int CardStock { get; set; }
        public string CardDesc { get; set; } = string.Empty;
        public string CardImg { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        // 导航属性
        public virtual Category? Category { get; set; }
        public virtual ICollection<UserCard> UserCards { get; set; } = new List<UserCard>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }

    public class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.PlayerName).IsRequired().HasMaxLength(100);

            // Ensure decimal precision for SQL Server to avoid truncation warnings
            builder.Property(c => c.CardPrice)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(c => c.CardStock)
                   .IsRequired()
                   .HasDefaultValue(0);

            // Remove store-generated configuration so EF includes UpdateTime in INSERT.
            // This prevents EF from omitting UpdateTime and causing a NULL insert on a NOT NULL DB column.
            builder.Property(c => c.UpdateTime)
                   .IsRequired();

            // Optional: set server default for CreateTime so DB will have a fallback if app doesn't set it.
            builder.Property(c => c.CreateTime)
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            builder.HasOne(c => c.Category)
                   .WithMany(c => c.Cards)
                   .HasForeignKey(c => c.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(c => c.PlayerName);
            builder.HasIndex(c => c.CategoryId);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;

namespace GGOnlineShop.Models
{
    public class Card
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }

        [Required] // 必填属性
        public string PlayerName { get; set; }

        public string Team { get; set; }
        public int CardYear { get; set; }
        public string CardGrade { get; set; }
        public decimal CardPrice { get; set; }
        public int CardStock { get; set; }
        public string CardDesc { get; set; }
        public string CardImg { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        // 导航属性
        public virtual Category Category { get; set; }
        public virtual ICollection<UserCard> UserCards { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
    }

    public class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.PlayerName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.CardPrice).IsRequired();
            builder.Property(c => c.CardStock).IsRequired().HasDefaultValue(0);
            builder.Property(c => c.UpdateTime).ValueGeneratedOnAddOrUpdate();

            builder.HasOne(c => c.Category)
                   .WithMany(c => c.Cards)
                   .HasForeignKey(c => c.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(c => c.PlayerName);
            builder.HasIndex(c => c.CategoryId);
        }
    }
}
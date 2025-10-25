using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GGOnlineShop.Models
{
    public class UserCard
    {
        public int Id { get; set; }
        public int UserId { get; set; } // 外键改为int，关联UserInfo.Id
        public int CardId { get; set; }
        public int HoldNum { get; set; } = 1;
        public bool IsSell { get; set; } = false;
        public decimal? SellPrice { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;

        // 导航属性（关联正确的主键类型）
        public virtual Card Card { get; set; } = null!;
        public virtual UserInfo User { get; set; } = null!;
    }

    public class UserCardConfiguration : IEntityTypeConfiguration<UserCard>
    {
        public void Configure(EntityTypeBuilder<UserCard> builder)
        {
            builder.HasKey(uc => uc.Id);
            builder.Property(uc => uc.HoldNum).IsRequired().HasDefaultValue(1);

            builder.HasOne(uc => uc.Card)
                   .WithMany(c => c.UserCards)
                   .HasForeignKey(uc => uc.CardId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(uc => uc.User)
                   .WithMany(u => u.UserCards)
                   .HasForeignKey(uc => uc.UserId) // 外键关联UserInfo.Id
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(uc => new { uc.UserId, uc.CardId }).IsUnique();
            builder.HasIndex(uc => uc.IsSell);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GGOnlineShop.Models
{
    [Table("UserInfos")]
    public class UserInfo
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        // Make Email nullable for safe migration
        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? Email { get; set; } = null;

        [MaxLength(200)]
        public string Avatar { get; set; } = string.Empty;

        [MaxLength(200)]
        public string AvatarUrl { get; set; } = string.Empty;

        [NotMapped]
        public IFormFile? AvatarFile { get; set; }

        public int? Age { get; set; }

        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        [MaxLength(50)]
        public string FavoriteTeam { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [NotMapped]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "两次输入的密码不一致")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool IsAdmin { get; set; } = false;

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<UserCard> UserCards { get; set; } = new List<UserCard>();
    }

    public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
    {
        public void Configure(EntityTypeBuilder<UserInfo> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
            builder.HasIndex(u => u.Username).IsUnique();

            // Email is optional at DB level initially
            builder.Property(u => u.Email).HasMaxLength(100).IsRequired(false);

            // Create a filtered unique index so NULL/empty values don't break it
            builder.HasIndex(u => u.Email)
                   .IsUnique()
                   .HasFilter("[Email] IS NOT NULL AND Email <> ''");

            builder.Property(u => u.IsAdmin).HasDefaultValue(false);

            builder.HasMany(u => u.UserCards)
                   .WithOne(uc => uc.User)
                   .HasForeignKey(uc => uc.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
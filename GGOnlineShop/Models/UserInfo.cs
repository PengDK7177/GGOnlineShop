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
        [Key] // 主键改为int类型
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty; // 从主键改为普通唯一字段

        [MaxLength(200)]
        public string Avatar { get; set; } = string.Empty;
        [MaxLength(200)]
        public string AvatarUrl { get; set; } = string.Empty;
        [NotMapped]
        public IFormFile? AvatarFile { get; set; } // 用于上传头像文件
        public int? Age { get; set; }

        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        [MaxLength(50)]
        public string FavoriteTeam { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        [DataType(DataType.Password)] // 标记为密码类型，页面会自动渲染为密码输入框
        public string Password { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "两次输入的密码不一致")] // 与Password字段比对
        [NotMapped] // 不映射到数据库（仅用于前端验证）
        public string ConfirmPassword { get; set; } = string.Empty;

        // 导航属性（初始化集合）
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<UserCard> UserCards { get; set; } = new List<UserCard>();
    }

    public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
    {
        public void Configure(EntityTypeBuilder<UserInfo> builder)
        {
            builder.HasKey(u => u.Id); // 主键为Id（int类型）
            builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
            builder.HasIndex(u => u.Username).IsUnique(); // 保持用户名唯一

            builder.HasMany(u => u.UserCards)
                   .WithOne(uc => uc.User)
                   .HasForeignKey(uc => uc.UserId) // 外键关联UserInfo.Id
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
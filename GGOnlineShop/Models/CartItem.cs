using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GGOnlineShop.Models
{
    public class CartItem
    {
        [Key]
        [Column(Order = 1)]
        public int CardId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int UserId { get; set; } // 外键改为int，关联UserInfo.Id

        [Required]
        public int Quantity { get; set; }

        // 导航属性
        [ForeignKey("CardId")]
        public virtual Card Card { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual UserInfo User { get; set; } = null!;
    }
}
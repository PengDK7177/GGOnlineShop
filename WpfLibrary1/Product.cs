using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace GGOnlineShop.Models
{
    public class Product
    {
        [Key] // 主键
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 自增ID
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; }

        [Column(TypeName = "decimal(18,2)")] // 价格精度（18位整数，2位小数）
        public decimal Price { get; set; }

        [MaxLength(200)]
        public string ImageUrl { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        // 导航属性：关联购物车项
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
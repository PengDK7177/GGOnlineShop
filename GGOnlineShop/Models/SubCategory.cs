using System.ComponentModel.DataAnnotations;

namespace GGOnlineShop.Models
{
    public class SubCategory
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}

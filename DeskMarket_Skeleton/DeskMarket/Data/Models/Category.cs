using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using static DeskMarket.Validation.Constants;

namespace DeskMarket.Data.Models
{
    public class Category
    {
        [Key]
        [Comment("Unique identifier of the category")]
        public int Id   { get; set; }

        [Required]
        [MaxLength(CategoryNameMaxLength)]
        public string Name { get; set; } = null!;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

using DeskMarket.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static DeskMarket.Validation.Constants;


namespace DeskMarket.Models
{
    public class ProductViewModel
    {
        [Required]
        [StringLength(ProductNameMaxLength, MinimumLength = ProductNameMinLength)]
        public string ProductName { get; set; } = null!;
        [Required]
        [StringLength(ProductDescriptionMaxLength, MinimumLength = ProductDescriptionMinLength)]
        public string Description { get; set; } = null!;
        [Required]
        [Precision(18, 2)]
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        [Required] 
        public string AddedOn { get; set; } = null!;
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string SellerId { get; set; } = null!;
        public List<Category> Categories { get; set; } = new List<Category>();


    }
}

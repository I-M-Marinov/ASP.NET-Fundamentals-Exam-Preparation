using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static DeskMarket.Validation.Constants;

namespace DeskMarket.Data.Models
{
    public class Product
    {
        [Key]
        [Comment("Unique identifier of the product")]
        public int Id { get; set; }

        [Required]
        [MaxLength(ProductNameMaxLength)]
        [Comment("Name of the product")]

        public string ProductName { get; set; } = null!;

        [Required]
        [MaxLength(ProductDescriptionMaxLength)]
        [Comment("Description of the product")]

        public string Description { get; set; } = null!;

        [Required]
        [Comment("Price of the product")]
        [Precision(18,2)]
        public decimal Price { get; set; }

        [Comment("Url of the image of the product")]
        public string? ImageUrl { get; set; }

        [Required]
        public string SellerId { get; set; } = null!;

        [ForeignKey(nameof(SellerId))]
        public IdentityUser Seller { get; set; } = null!;

        [Required]
        [Comment("The date and time the product was added on")]
        public DateTime AddedOn { get; set; } = DateTime.Now; // default to DateTime.Now 

        [Required]
        public int CategoryId { get; set; } 

        [ForeignKey(nameof(CategoryId))]

        public Category Category { get; set; } = null!;

        [Comment("Shows if the product is deleted or not")]
        public bool IsDeleted { get; set; } = false; // default to false

        public ICollection<ProductClient> ProductsClients { get; set; } = new List<ProductClient>();
    }
}

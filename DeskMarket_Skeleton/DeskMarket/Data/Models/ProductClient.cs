using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeskMarket.Data.Models
{
    [PrimaryKey(nameof(ProductId), nameof(ClientId))]

    public class ProductClient
    {
        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))] 
        public Product Product { get; set; } = null!;

        [Required]
        public string ClientId { get; set; } = null!;

        [ForeignKey(nameof(ClientId))]
        public IdentityUser Client { get; set; } = null!;
    }
}

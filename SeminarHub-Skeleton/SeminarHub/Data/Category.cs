using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using static SeminarHub.ValidationConstants.Constants;


namespace SeminarHub.Data
{
    public class Category
    {
        [Key]
        [Comment("Unique Category Identifier")]
        public int Id { get; set; }

        [Required]
        [Comment("Name of the Category")]
        [MaxLength(CategoryNameMaxLength)]
        public string Name { get; set; } = null!;

        public ICollection<Seminar> Seminars { get; set; } = new List<Seminar>();
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using static Homies.Validation.Constants;


namespace Homies.Data
{
    public class Type
    {
        [Key]
        [Comment("Unique identifier of the Type")]
        public int Id { get; set; }

        [Required]
        [MaxLength(TypeNameMaxLength)]
        public string Name { get; set; } = null!;

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}

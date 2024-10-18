using Type = Homies.Data.Type;
using static Homies.Validation.Constants;
using System.ComponentModel.DataAnnotations;


namespace Homies.Models
{
    public class EventViewModel
    {
        [Required]
        [StringLength(EventNameMaxLength, MinimumLength = EventNameMinLength)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(EventDescriptionMaxLength, MinimumLength = EventDescriptionMinLength)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string CreatedOn { get; set; } = DateTime.Now.ToString(DateTimeFormat); // set the creation date and time to NOW 

        [Required]
        public string Start { get; set; } = DateTime.Today.ToString(DateTimeFormat);

        [Required]
        public string End { get; set; } = DateTime.Today.ToString(DateTimeFormat);

        [Required]
        public int TypeId { get; set; }

        public List<Type> Types { get; set; } = new List<Type>();
    }
}
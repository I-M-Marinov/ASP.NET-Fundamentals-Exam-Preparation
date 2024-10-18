using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Homies.Validation.Constants;

namespace Homies.Data
{
    public class Event
    {
        [Key]
        [Comment("Unique identifier of the Event")]

        public int Id { get; set; }

        [Required]
        [Comment("Name of the Event")]
        [MaxLength(EventNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [Comment("Description of the Event")]
        [MaxLength(EventDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [Required] 
        public string OrganiserId { get; set; } = null!;

        [ForeignKey(nameof(OrganiserId))]
        public IdentityUser Organiser { get; set; } = null!;

        [Required]
        [Comment("The date the Event was created on")]
        public DateTime CreatedOn { get; set; }

        [Required]
        [Comment("Date and time when the Event starts")]
        public DateTime Start { get; set; }

        [Required]
        [Comment("Date and time when the Event ends")]
        public DateTime End { get; set; }

        [Required]
        public int TypeId { get; set; }

        [ForeignKey(nameof(TypeId))] 
        public Type Type { get; set; } = null!;

        [Comment("Shows if the Event is deleted or not")]
        public bool IsDeleted { get; set; } // Soft Delete Implementation 

        public ICollection<EventParticipant> EventsParticipants { get; set; } = new List<EventParticipant>();


    }
}

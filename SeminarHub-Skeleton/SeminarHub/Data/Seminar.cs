using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static SeminarHub.ValidationConstants.Constants;

namespace SeminarHub.Data
{
    public class Seminar
    {
        [Key]
        [Comment("Unique Identifier of the seminar")]
        public int Id { get; set; }

        [Required]
        [Comment("Topic of the seminar")]
        [MaxLength(TopicMaxLength)] 
        public string Topic { get; set; } = null!;

        [Required]
        [Comment("The lecturer of the seminar")]
        [MaxLength(LecturerMaxLength)]
        public string Lecturer { get; set; } = null!;

        [Required]
        [Comment("The details of the seminar")]
        [MaxLength(DetailsMaxLength)]
        public string Details { get; set; } = null!;

        [Required]
        public string OrganizerId { get; set; } = null!;

        [ForeignKey(nameof(OrganizerId))]
        public IdentityUser Organizer { get; set; } = null!;

        [Required]
        [Comment("Date and time of the seminar")]
        public DateTime DateAndTime { get; set; }

        [Comment("Duration of the seminar")]
        [Range(DurationMinValue, DurationMaxValue)]
        public int? Duration { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        public ICollection<SeminarParticipant> SeminarsParticipants { get; set; } = new List<SeminarParticipant>();
    }
}

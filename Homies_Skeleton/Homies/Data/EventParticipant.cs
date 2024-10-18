using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

namespace Homies.Data
{
    [PrimaryKey(nameof(HelperId), nameof(EventId))]
    public class EventParticipant
    {
        [Required] 
        public string HelperId { get; set; } = null!;

        [ForeignKey(nameof(HelperId))]
        public IdentityUser Helper { get; set; } = null!;

        [Required]
        public int EventId { get; set; }

        [ForeignKey(nameof(EventId))]
        public Event Event { get; set; } = null!;
    }
}

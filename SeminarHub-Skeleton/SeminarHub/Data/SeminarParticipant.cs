using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeminarHub.Data
{
    [PrimaryKey(nameof(SeminarId), nameof(ParticipantId))]
    public class SeminarParticipant
    {
        public int SeminarId { get; set; }

        [ForeignKey(nameof(SeminarId))]

        public Seminar Seminar { get; set; }

        public string ParticipantId { get; set; }

        [ForeignKey(nameof(ParticipantId))]

        public IdentityUser Participant { get; set; }
    }
}

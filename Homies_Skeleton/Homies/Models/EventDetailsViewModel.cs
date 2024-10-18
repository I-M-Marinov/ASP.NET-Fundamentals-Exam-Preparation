using System.Runtime.Serialization;

namespace Homies.Models
{
    public class EventDetailsViewModel
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Start { get; set; }

        public required string End { get; set; }

        public required string CreatedOn { get; set; } 

        public required string Description { get; set; }

        public required string Type { get; set; }

        public required string Organiser { get; set; }
    }
}

namespace Homies.Models
{
    public class EventInfoViewModel
    {
        public int Id { get; set; }

        public required  string Name { get; set; }

        public required string Start { get; set; }

        public required string Type { get; set; }

        public required string Organiser { get; set; }

    }
}

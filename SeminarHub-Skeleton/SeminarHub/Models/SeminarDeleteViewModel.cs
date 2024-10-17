namespace SeminarHub.Models
{
    public class SeminarDeleteViewModel
    {
        public required int Id { get; set; }

        public required string Topic { get; set; } = null!;

        public required DateTime DateAndTime { get; set; } 
    }
}

namespace GameZone.Models
{
    public class DeleteGameViewModel
    {

        public int Id { get; set; }

        public required string Title { get; set; }

        public required string Publisher { get; set; }
    }
}

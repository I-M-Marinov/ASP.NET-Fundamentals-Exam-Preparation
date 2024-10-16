using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using static GameZone.Constants.ValidationConstants;


namespace GameZone.Data
{
	public class Genre
	{
		[Key]
		[Comment("Unique Genre Identifier")]
		public int Id { get; set; }

		[Required]
		[Comment("Name of the Genre")]
		[MaxLength(GenreNameMaxLength)]
		public string Name { get; set; }

		public ICollection<Game> Games { get; set; } = new List<Game>();
	}
}

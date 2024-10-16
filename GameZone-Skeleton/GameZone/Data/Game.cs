using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static GameZone.Constants.ValidationConstants;

namespace GameZone.Data
{
	public class Game
	{
		[Key]
		[Comment("Unique Identifier")]
		public int Id { get; set; }

		[Required]
		[MaxLength(GameTitleMaxLength)]
		[Comment("Title of the game")]

		public string Title { get; set; } = null!;

		[Required]
		[MaxLength(GameDescriptionMaxLength)]
		[Comment("Description of the game")]
		public string Description { get; set; } = null!;

		[Comment("The url of the image of the game")]
		public string? ImageUrl { get; set; }

		[Required]
		[Comment("Identifier of the game publisher")]

		public string PublisherId { get; set; } = null!;

		[ForeignKey(nameof(PublisherId))]
		public IdentityUser Publisher { get; set; } = null!;

		[Required]
		[Comment("Release date of the game")]
		public DateTime ReleasedOn { get; set; } // DateTime with format " yyyy-MM-dd"

		[Required]
		[Comment("Genre of the game")]
		public int GenreId { get; set; }

		[ForeignKey(nameof(GenreId))]
		public Genre Genre { get; set; } = null!;

		public ICollection<GamerGame> GamersGames { get; set; } = new List<GamerGame>();

		[Comment("Shows if the game is deleted or not")]
		public bool IsDeleted { get; set; }

	}
}

using System.Security.Policy;

namespace GameZone.Constants
{
	public static class ValidationConstants
    {

        public const byte GameTitleMinLength = 2;
		public const byte GameTitleMaxLength = 50;
		public const int GameDescriptionMinLength = 10;
		public const int GameDescriptionMaxLength = 500;
		public const byte GenreNameMaxLength = 25;
        public const string GameReleasedOnFormat = "yyyy-MM-dd";

    }
}

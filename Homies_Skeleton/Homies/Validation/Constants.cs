using Microsoft.EntityFrameworkCore.Storage.Json;

namespace Homies.Validation
{
    public static class Constants
    {
        // EVENT

        public const byte EventNameMinLength = 5;
        public const byte EventNameMaxLength = 20;

        public const byte EventDescriptionMinLength = 15;
        public const byte EventDescriptionMaxLength = 150;

        public const string DateTimeFormat = "yyyy-MM-dd H:mm";
        public static readonly string[] AcceptedDateTimeFormats = { "yyyy-MM-dd H:mm", "dd/MM/yyyy H:mm" };
        // TYPE 

        public const byte TypeNameMinLength = 5;
        public const byte TypeNameMaxLength = 15;


    }
}

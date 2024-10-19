namespace DeskMarket.Validation
{
    public static class Constants
    {
        public const byte ProductNameMinLength = 2;
        public const byte ProductNameMaxLength = 60;
        public const byte ProductDescriptionMinLength = 10;
        public const byte ProductDescriptionMaxLength = 250;
        public const decimal ProductPriceMinValue = 1.00m;
        public const decimal ProductPriceMaxValue = 3000.00m;
        public const string AddedOnFormat = "dd-MM-yyyy";
        public const byte CategoryNameMaxLength = 20;
    }
}

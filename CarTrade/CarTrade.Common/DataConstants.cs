namespace CarTrade.Common
{
    public class DataConstants
    {
        public const int BrandMinLength = 2;
        public const int BrandMaxLength = 50;

        public const int VehicleModelTypeMinLength = 2;
        public const int VehicleModelTypeMaxLength = 50;

        public const int PlateNumberMinLength = 7;
        public const int PlateNumberМaxLength = 12;

        public const int VinMinLength = 11;
        public const int VinMaxLength = 17;

        public const int MinNameLength = 2;
        public const int MaxNameLength = 50;

        public const int DescriptionLength = 200;

        public const int PicUrlLength = 300;

        public const int DaysBeforeItExpires = 30;
        public const int RemainDistanceOilChange = 1000;

        public const string NotExistItemExceptionMessage = "missing item";
        public const string ExistItemExceptionMessage = "This item exist or it is active";
        public const string WrongDateExceptionMessage = "Start date must be small than end date" +
                    "or the date must not be less than one year back";
    }
}

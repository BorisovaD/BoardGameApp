namespace BoardGameApp.Web.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class ValidationMessages
    {
        public static class Club
        {
            public const string NameRequiredMessage = "Club name is required.";
            public const string NameMinLengthMessage = "Club name must be at least 5 characters.";
            public const string NameMaxLengthMessage = "Club name cannot exceed 80 characters.";

            public const string AddressRequiredMessage = "Address is required.";
            public const string AddressMinLengthMessage = "Address name must be at least 8 characters.";
            public const string AddressMaxLengthMessage = "Address name cannot exceed 100 characters.";

            public const string CityRequiredMessage = "City name is required.";
            public const string CityMinLengthMessage = "City name must be at least 3 characters long.";
            public const string CityMaxLengthMessage = "City name cannot exceed 50 characters.";

        }
    }
}

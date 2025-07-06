namespace BoardGameApp.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class EntityConstants
    {
        public static class City
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 50;
        }

        public static class Club
        {
            public const int NameMinLength = 5;
            public const int NameMaxLength = 80;

            public const int AddressMinLength = 8;
            public const int AddressMaxLength = 100;
        }
    }
}

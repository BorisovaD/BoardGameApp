namespace BoardGameApp.Web.ViewModels.Admin.ClubManagement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static BoardGameApp.Data.Common.EntityConstants.Club;
    using static BoardGameApp.Data.Common.EntityConstants.City;
    using static BoardGameApp.Web.ViewModels.ValidationMessages.Club;
    using BoardGameApp.Data.Common;

    public class ClubManagementCreateInputModel
    {
        [Required(ErrorMessage = NameRequiredMessage)]
        [MinLength(EntityConstants.Club.NameMinLength, ErrorMessage = NameMinLengthMessage)]
        [MaxLength(EntityConstants.Club.NameMaxLength, ErrorMessage = NameMaxLengthMessage)]
        public string ClubName { get; set; } = null!;

        [Required(ErrorMessage = AddressRequiredMessage)]
        [MinLength(AddressMinLength, ErrorMessage = AddressMinLengthMessage)]
        [MaxLength(AddressMaxLength, ErrorMessage = AddressMaxLengthMessage)]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = CityRequiredMessage)]
        [MinLength(EntityConstants.City.NameMinLength, ErrorMessage = CityMinLengthMessage)]
        [MaxLength(EntityConstants.City.NameMaxLength, ErrorMessage = CityMaxLengthMessage)]
        public string CityName { get; set; } = null!;        
    }
}

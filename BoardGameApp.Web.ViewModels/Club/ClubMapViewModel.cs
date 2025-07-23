namespace BoardGameApp.Web.ViewModels.Club
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ClubMapViewModel
    {
        public Guid ClubId { get; set; }
        public string ClubName { get; set; } = null!;
        public Guid CityId { get; set; }
        public string CityName { get; set; } = null!;
    }
}

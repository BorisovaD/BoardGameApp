namespace BoardGameApp.Web.ViewModels.Admin.ClubManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ClubManagementIndexViewModel
    {
        public Guid Id { get; set; }

        public string ClubName { get; set; } = null!;

        public string CityName { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public string? ManagerName { get; set; }
    }
}

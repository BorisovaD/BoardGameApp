namespace BoardGameApp.Web.ViewModels.Admin.ClubManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ClubManagementEditInputModel : ClubManagementCreateInputModel
    {
        public Guid Id { get; set; }
    }
}

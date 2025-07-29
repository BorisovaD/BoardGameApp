namespace BoardGameApp.Web.ViewModels.Admin.UserManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UserManagementIndexViewModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public IEnumerable<string> Roles { get; set; } = null!;
    }
}

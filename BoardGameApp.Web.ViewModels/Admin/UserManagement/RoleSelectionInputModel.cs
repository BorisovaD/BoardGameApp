namespace BoardGameApp.Web.ViewModels.Admin.UserManagement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RoleSelectionInputModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Role { get; set; } = null!;
    }
}

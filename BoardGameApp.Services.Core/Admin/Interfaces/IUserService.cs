namespace BoardGameApp.Services.Core.Admin.Interfaces
{
    using BoardGameApp.Web.ViewModels.Admin.UserManagement;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IUserService
    {
        Task<IEnumerable<UserManagementIndexViewModel>> GetUserManagementInfoAsync(Guid userId);

        Task<bool> AssignRoleAsync(RoleSelectionInputModel inputModel);

        Task<bool> RemoveRoleAsync(RoleSelectionInputModel inputModel);

        Task<bool> DeleteUserByIdAsync(Guid userId);
    }
}

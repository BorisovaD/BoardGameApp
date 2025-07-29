namespace BoardGameApp.Services.Core.Admin.Interfaces
{
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.Admin.ClubManagement;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IClubManagementService : IClubService
    {
        Task<IEnumerable<ClubManagementIndexViewModel>> GetClubManagementInfoAsync();

        Task<bool> AddClubAsync(ClubManagementCreateInputModel? inputModel);

        Task<ClubManagementEditInputModel?> GetClubForEditingAsync(Guid id);

        Task<bool> EditClubAsync(ClubManagementEditInputModel? inputModel);

        Task<(bool Success, bool IsNowDeleted)> ToggleClubDeletionAsync(Guid id);
    }
}

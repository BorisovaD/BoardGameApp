namespace BoardGameApp.Services.Core.Contracts
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Web.ViewModels.Club;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IClubService
    {
        Task<IEnumerable<ClubMapViewModel>> GetAllActiveClubs();
    }
}

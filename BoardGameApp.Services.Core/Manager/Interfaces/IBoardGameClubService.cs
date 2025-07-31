namespace BoardGameApp.Services.Core.Manager.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IBoardGameClubService
    {
        Task ToggleGameInClubAsync(Guid clubId, Guid gameId);
    }
}

namespace BoardGameApp.Services.Core.Contracts
{
    using BoardGameApp.Web.ViewModels.GameSession;
    using BoardGameApp.Web.ViewModels.Reservation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IReservationService
    {
        Task<IEnumerable<ReservationIndexViewModel>> GetAllActiveReservations();
        Task<IEnumerable<ReservationIndexViewModel>> GetMyReservationsAsync(Guid userId);
    }
}

namespace BoardGameApp.Services.Core.Contracts
{
    using BoardGameApp.Web.ViewModels.BoardGame;
    using BoardGameApp.Web.ViewModels.Reservation;
    using BoardGameApp.Web.ViewModels.Ticket;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ITicketService
    {
        Task<IEnumerable<TicketIndexViewModel>> GetAllTickets();

        Task<TicketDetailsViewModel> GetTicketDetailsAsync(Guid? id, Guid? userId);
    }
}

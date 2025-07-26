namespace BoardGameApp.Services.Core
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.BoardGame;
    using BoardGameApp.Web.ViewModels.Reservation;
    using BoardGameApp.Web.ViewModels.Ticket;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TicketService : ITicketService
    {
        private readonly IRepository<Ticket> ticketRepository;
        public TicketService(IRepository<Ticket> ticketRepository)
        {
            this.ticketRepository = ticketRepository;
        }
        public async Task<IEnumerable<TicketIndexViewModel>> GetAllTickets()
        {
            TicketIndexViewModel[]? tickets = await ticketRepository
                .All()
                .Include(t => t.Reservation)
                .ThenInclude(r => r.GameSession)
                .ThenInclude(gs => gs.BoardGame)
                .Include(t => t.User)
                .Select(t => new TicketIndexViewModel
                {
                    Id = t.Id,
                    IssuedOn = t.IssuedOn,
                    UserName = t.User.UserName!,
                    Price = t.Price,
                    Quantity = t.Quantity,
                    ReservationId = t.ReservationId,
                    BoardGameTitle = t.Reservation.GameSession.BoardGame.Title,
                    
                })
                .ToArrayAsync();

            return tickets;
        }

        public async Task<TicketDetailsViewModel> GetTicketDetailsAsync(Guid? id, Guid? userId)
        {
            TicketDetailsViewModel? detailsVm = null;

            if (id.HasValue)
            {
                Ticket? ticketModel = await ticketRepository
                    .All()
                    .Include(t => t.User)
                    .Include(t => t.Reservation)
                    .ThenInclude(t => t.GameSession)
                    .ThenInclude(t => t.BoardGame)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(g => g.Id == id.Value);

                if (ticketModel != null)
                {
                    detailsVm = new TicketDetailsViewModel()
                    {
                        Id = ticketModel.Id,
                        IssuedOn = ticketModel.IssuedOn,
                        Price = ticketModel.Price,
                        Quantity = ticketModel.Quantity,
                        ReservationId = ticketModel.ReservationId,
                        ReservationTime = ticketModel.Reservation.ReservationTime,
                        GameSessionId = ticketModel.Reservation.GameSessionId,
                        StartTime = ticketModel.Reservation.GameSession.StartTime,
                        EndTime = ticketModel.Reservation.GameSession.EndTime,
                        BoardGameTitle = ticketModel.Reservation.GameSession.BoardGame.Title,
                        UserName = ticketModel.User.UserName!                        
                    };
                }
            }

            return detailsVm!;
        }
    }
}

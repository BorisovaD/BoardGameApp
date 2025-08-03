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
        private readonly IRepository<GameSession> gameSessionRepository;
        private readonly IRepository<Reservation> reservationRepository;
        public TicketService(IRepository<Ticket> ticketRepository, IRepository<GameSession> gameSessionRepository, IRepository<Reservation> reservationRepository)
        {
            this.ticketRepository = ticketRepository;
            this.gameSessionRepository = gameSessionRepository;
            this.reservationRepository = reservationRepository;
        }

        public async Task<bool> BuyTicketAsync(Guid userId, Guid gameSessionId, int ticketsToBuy)
        {
            GameSession? session = await gameSessionRepository
                .All()
                .Include(gs => gs.Reservations)
                .FirstOrDefaultAsync(gs => gs.Id == gameSessionId);

            if (session == null)
                return false;

            IEnumerable<Guid> reservationIds = session.Reservations.Select(r => r.Id).ToList();

            
            int boughtTickets = await ticketRepository.All()
                .Where(t => reservationIds.Contains(t.ReservationId))
                .SumAsync(t => (int?)t.Quantity) ?? 0;

            if (boughtTickets + ticketsToBuy > session.MaxPlayers)
                return false;

            
            Reservation? userReservation = session.Reservations.FirstOrDefault(r => r.UserId == userId);

            if (userReservation == null)
            {
                userReservation = new Reservation
                {
                    Id = Guid.NewGuid(),
                    ReservationTime = DateTime.UtcNow,
                    UserId = userId,
                    GameSessionId = gameSessionId
                };

                await reservationRepository.AddAsync(userReservation);
                await reservationRepository.SaveChangesAsync();
            }

            var newTicket = new Ticket
            {
                Id = Guid.NewGuid(),
                IssuedOn = DateTime.UtcNow,
                Price = 15.00m, 
                Quantity = ticketsToBuy,
                UserId = userId,
                ReservationId = userReservation.Id
            };

            await ticketRepository.AddAsync(newTicket);
            await ticketRepository.SaveChangesAsync();

            return true;
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

        public async Task<BuyTicketViewModel> GetTicketInfoAsync(Guid gameSessionId)
        {
            IEnumerable<Guid> reservationIds = await reservationRepository
                .All()
                .Where(r => r.GameSessionId == gameSessionId)
                .Select(r => r.Id)
                .ToListAsync();

            int boughtTickets = await ticketRepository
                .All()
                .Where(t => reservationIds.Contains(t.ReservationId))
                .SumAsync(t => t.Quantity);

            BuyTicketViewModel? gameSession = await gameSessionRepository
                .All()
                .Where(gs => gs.Id == gameSessionId && gs.IsDeleted == false)
                .Select(gs => new BuyTicketViewModel
                {
                    GameSessionId = gs.Id,
                    GameTitle = gs.BoardGame.Title,
                    AvailableTickets = gs.MaxPlayers - boughtTickets
                })
                .FirstOrDefaultAsync();

            return gameSession!;
        }
    }
}

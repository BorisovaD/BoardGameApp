namespace BoardGameApp.Services.Core
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.GameSession;
    using BoardGameApp.Web.ViewModels.Reservation;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ReservationService : IReservationService
    {
        private readonly IRepository<Reservation> reservationRepository;
        public ReservationService(IRepository<Reservation> reservationRepository)
        {
            this.reservationRepository = reservationRepository;
        }
        public async Task<IEnumerable<ReservationIndexViewModel>> GetAllActiveReservations()
        {
            ReservationIndexViewModel[]? reservations = await reservationRepository
                .All()
                .Include(r => r.GameSession)
                    .ThenInclude(gs => gs.BoardGame)
                .Include(r => r.User)
                .Select(r => new ReservationIndexViewModel
                {
                    Id = r.Id,
                    ReservationTime = r.ReservationTime,
                    UserEmail = r.User.Email!,
                    GameSessionId = r.GameSessionId,
                    SessionStartTime = r.GameSession.StartTime,
                    BoardGameId = r.GameSession.BoardGame.Id,
                    BoardGameTitle = r.GameSession.BoardGame.Title,
                    ImageUrl = r.GameSession.BoardGame.ImageUrl,
                    Tickets = r.User.Tickets
                        .Where(t => t.ReservationId == r.Id)
                        .Sum(t => t.Quantity)
                })
                .ToArrayAsync();

            return reservations;
        }

        public async Task<IEnumerable<ReservationIndexViewModel>> GetMyReservationsAsync(Guid userId)
        {
            return await reservationRepository
                .All()
                .Where(r => r.UserId == userId)
                .Include(r => r.User) 
                .ThenInclude(u => u.Tickets) 
                .Include(r => r.GameSession)
                .ThenInclude(gs => gs.BoardGame)
                .Select(r => new ReservationIndexViewModel
                {
                    Id = r.Id,
                    ReservationTime = r.ReservationTime,
                    UserEmail = r.User.Email!,
                    GameSessionId = r.GameSessionId,
                    SessionStartTime = r.GameSession.StartTime,
                    BoardGameId = r.GameSession.BoardGame.Id,
                    BoardGameTitle = r.GameSession.BoardGame.Title,
                    ImageUrl = r.GameSession.BoardGame.ImageUrl,
                    TicketId = r.User.Tickets
                        .Where(t => t.ReservationId == r.Id)
                        .Select(t => t.Id)
                        .FirstOrDefault(),
                    Tickets = r.User.Tickets
                        .Where(t => t.ReservationId == r.Id)
                        .Sum(t => t.Quantity)
                })
                .ToArrayAsync();
        }
    }
}

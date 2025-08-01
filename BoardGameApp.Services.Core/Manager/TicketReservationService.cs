namespace BoardGameApp.Services.Core.Manager
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Manager.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TicketReservationService : ITicketReservationService
    {
        private readonly IRepository<GameSession> gameSessionRepository;

        public TicketReservationService(IRepository<GameSession> gameSessionRepository)
        {
            this.gameSessionRepository = gameSessionRepository;
        }
        public async Task<bool> UpdateMaxPlayersAsync(Guid gameSessionId, int newMaxPlayers)
        {
            GameSession? session = await gameSessionRepository.GetByIdAsync(gameSessionId);

            if (session == null || session.IsDeleted || session.CurrentPlayers > 0)
            {
                return false; 
            }

            session.MaxPlayers = newMaxPlayers;

            await gameSessionRepository.SaveChangesAsync();

            return true;
        }
    }
}

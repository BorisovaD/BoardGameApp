namespace BoardGameApp.Services.Core
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.GameSession;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GameSessionService : IGameSessionService
    {
        private readonly IRepository<GameSession> gameSessionRepository;

        public GameSessionService(IRepository<GameSession> gameSessionRepository)
        {
            this.gameSessionRepository = gameSessionRepository;
        }
        public async Task<IEnumerable<GameSessionsViewModel>> GetAllActiveGameSessions()
        {
            GameSessionsViewModel[]? sessions = await gameSessionRepository
                .All()
                .Where(s => !s.IsDeleted && s.StartTime > DateTime.Now)
                .Include(s => s.BoardGame)
                .Include(s => s.Club)
                .ThenInclude(c => c.City)
                .Select(s => new GameSessionsViewModel
                {
                    Id = s.Id,
                    BoardGameName = s.BoardGame.Title,
                    ClubName = s.Club.Name,
                    CityName = s.Club.City.Name, 
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    MaxPlayers = s.MaxPlayers,
                    CurrentPlayers = s.CurrentPlayers,
                    ImageUrl = s.BoardGame.ImageUrl
                })
                .ToArrayAsync();

            return sessions;
        }
    }
}

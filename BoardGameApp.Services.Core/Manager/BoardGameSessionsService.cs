namespace BoardGameApp.Services.Core.Manager
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Manager.Interfaces;
    using BoardGameApp.Web.ViewModels.Manager.GameSessions;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BoardGameSessionsService : IBoardGameSessionsService
    {
        private readonly IRepository<GameSession> gameSessionRepository;
        private readonly IRepository<ClubBoardGame> clubBoardGameRepository;
        public BoardGameSessionsService(IRepository<GameSession> gameSessionRepository, IRepository<ClubBoardGame> clubBoardGameRepository)
        {
            this.gameSessionRepository = gameSessionRepository;
            this.clubBoardGameRepository = clubBoardGameRepository;
        }
        public async Task<IEnumerable<ManageGameSessionViewModel>> GetManageViewModelAsync(Guid clubId)
        {
            IEnumerable<ClubBoardGame> games = await clubBoardGameRepository
                .All()
                .Where(g => !g.IsDeleted && g.ClubId == clubId)
                .Include(g => g.BoardGame) 
                .ToListAsync();

            IEnumerable<GameSession> sessions = await gameSessionRepository
                .All()
                .Where(s => s.ClubId == clubId)
                .ToListAsync();

            IEnumerable<ManageGameSessionViewModel> result = games.Select(game =>
            {
                GameSession? session = sessions.FirstOrDefault(s => s.BoardGameId == game.BoardGameId);
                return new ManageGameSessionViewModel
                {
                    ClubId = clubId,
                    BoardGameId = game.BoardGameId,
                    BoardGameTitle = game.BoardGame.Title,
                    StartTime = session?.StartTime,
                    EndTime = session?.EndTime
                };
            }).ToList();

            return result;
        }

        public async Task<bool> SaveGameSessionTimeAsync(Guid gameId, int startHour, int endHour)
        {
            System.DateTime today = DateTime.Today;
            System.DateTime startTime = today.AddHours(startHour);
            System.DateTime endTime = today.AddHours(endHour);

            GameSession? session = await gameSessionRepository.FirstOrDefaultAsync(s => s.BoardGameId == gameId);

            if (session == null)
            {
                session = new GameSession
                {
                    BoardGameId = gameId,
                    StartTime = startTime,
                    EndTime = endTime
                };

                await gameSessionRepository.AddAsync(session);
            }
            else
            {
                session.StartTime = startTime;
                session.EndTime = endTime;
            }

            await gameSessionRepository.SaveChangesAsync(); 

            return true;
        }
    }
}

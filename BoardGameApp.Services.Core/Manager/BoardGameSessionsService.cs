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

        public async Task<Guid> SaveGameSessionAsync(Guid boardGameId, DateTime startTime, DateTime endTime, Guid organizerId)
        {            
            ClubBoardGame? clubBoardGame = await this.clubBoardGameRepository
                .All()
                .Where(cb => cb.IsDeleted == false)
                .Include(cb => cb.BoardGame) 
                .FirstOrDefaultAsync(cb => cb.BoardGameId == boardGameId);

            if (clubBoardGame == null || clubBoardGame.BoardGame == null)
            {
                throw new InvalidOperationException("Game not found in club or game data missing.");
            }

            GameSession? existingSession = await this.gameSessionRepository
                .All()
                .FirstOrDefaultAsync(s => s.BoardGameId == boardGameId && s.ClubId == clubBoardGame.ClubId && s.IsDeleted == false);

            if (existingSession != null)
            {
                existingSession.StartTime = startTime;
                existingSession.EndTime = endTime;
                existingSession.IsDeleted = false;

                this.gameSessionRepository.Update(existingSession);
                await this.gameSessionRepository.SaveChangesAsync();

                return existingSession.Id;
            }
            else
            {
                var newSession = new GameSession
                {
                    Id = Guid.NewGuid(),
                    BoardGameId = boardGameId,
                    ClubId = clubBoardGame.ClubId,
                    StartTime = startTime,
                    EndTime = endTime,
                    OrganizerId = organizerId,
                    MaxPlayers = clubBoardGame.BoardGame.MaxPlayers,
                    CurrentPlayers = 0,
                    IsDeleted = false,
                };

                await this.gameSessionRepository.AddAsync(newSession);
                await this.gameSessionRepository.SaveChangesAsync();

                return newSession.Id;
            }            
        }        
    }
}

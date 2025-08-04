namespace BoardGameApp.Services.Core.Manager
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Manager.Interfaces;
    using BoardGameApp.Web.ViewModels.Manager.GameSessions;
    using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly IRepository<BoardGame> boardGameRepository;
        private readonly IRepository<Club> clubRepository;

        public BoardGameSessionsService(IRepository<GameSession> gameSessionRepository, IRepository<ClubBoardGame> clubBoardGameRepository, IRepository<BoardGame> boardGameRepository, IRepository<Club> clubRepository)
        {
            this.gameSessionRepository = gameSessionRepository;
            this.clubBoardGameRepository = clubBoardGameRepository;
            this.boardGameRepository = boardGameRepository;
            this.clubRepository = clubRepository;
        }

        public async Task<Guid> AddGameSessionAsync(AddGameSessionViewModel model, Guid organizerId)
        {
            GameSession? session = new GameSession
            {
                Id = Guid.NewGuid(),
                StartTime = DateTime.Today.AddHours(model.StartHour),
                EndTime = DateTime.Today.AddHours(model.EndHour),
                MaxPlayers = model.MaxPlayers,
                CurrentPlayers = 0,
                BoardGameId = model.BoardGameId,
                ClubId = model.ClubId,
                OrganizerId = organizerId,
                IsDeleted = false,
            };

            await gameSessionRepository.AddAsync(session);
            await gameSessionRepository.SaveChangesAsync();

            return session.Id;
        }

        public async Task EditGameSessionAsync(GameSessionEditModel model)
        {
            GameSession? session = await gameSessionRepository
                .All()
                .FirstOrDefaultAsync(gs => gs.Id == model.Id);

            if (session == null)
            {
                throw new Exception("Game session not found.");
            }

            session.StartTime = DateTime.Today.AddHours(model.StartTime);
            session.EndTime = DateTime.Today.AddHours(model.EndTime);
            session.MaxPlayers = model.MaxPlayers;
            session.BoardGameId = model.BoardGameId;
            session.ClubId = model.ClubId;

            await gameSessionRepository.SaveChangesAsync();
        }

        public async  Task<IEnumerable<SelectListItem>> GetAllBoardGamesAsync()
        {
            return await boardGameRepository
                .All()
                .Where(bg => bg.IsDeleted == false)
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.Title,
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetAllClubsAsync()
        {
            return await clubRepository
                .All()
                .Where(bg => bg.IsDeleted == false)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();
        }

        public async Task<GameSessionEditModel?> GetGameSessionForEditAsync(Guid id)
        {
            GameSessionEditModel? session = await gameSessionRepository
                .All()
                .Where(gs => gs.Id == id && !gs.IsDeleted)
                .Select(gs => new GameSessionEditModel
                {
                    Id = gs.Id,
                    StartTime = gs.StartTime.Hour,
                    EndTime = gs.EndTime.Hour,
                    MaxPlayers = gs.MaxPlayers,
                    BoardGameId = gs.BoardGameId,
                    ClubId = gs.ClubId,
                })
                .FirstOrDefaultAsync();

            if (session == null)
            {
                return null;
            }

            session.BoardGames = await GetAllBoardGamesAsync();
            session.Clubs = await GetAllClubsAsync();

            return session;
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
                    Id = session?.Id ?? Guid.Empty,
                    ClubId = clubId,
                    BoardGameId = game.BoardGameId,
                    BoardGameTitle = game.BoardGame.Title,
                    StartTime = session?.StartTime,
                    EndTime = session?.EndTime
                };
            }).ToList();

            return result;
        }

        public async Task<Guid> SaveGameSessionAsync(Guid boardGameId, DateTime startTime, DateTime endTime, Guid organizerId, bool isActive)
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
                existingSession.IsDeleted = !isActive;

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
                    IsDeleted = !isActive,
                };

                await this.gameSessionRepository.AddAsync(newSession);
                await this.gameSessionRepository.SaveChangesAsync();

                return newSession.Id;
            }            
        }        
    }
}

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

    public class BoardGameClubService : IBoardGameClubService
    {
        private readonly IRepository<ClubBoardGame> clubBoardGameRepository;

        public BoardGameClubService(IRepository<ClubBoardGame> clubBoardGameRepository)
        {
            this.clubBoardGameRepository = clubBoardGameRepository;
        }

        public async Task ToggleGameInClubAsync(Guid clubId, Guid gameId)
        {
            var existing = clubBoardGameRepository
                 .All()
                 .FirstOrDefault(x => x.ClubId == clubId && x.BoardGameId == gameId);

            if (existing != null)
            {
                clubBoardGameRepository.Delete(existing);
            }
            else
            {
                var newEntry = new ClubBoardGame
                {
                    ClubId = clubId,
                    BoardGameId = gameId
                };

                await clubBoardGameRepository.AddAsync(newEntry); 
            }

            await clubBoardGameRepository.SaveChangesAsync();
        }
    }
}

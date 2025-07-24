namespace BoardGameApp.Data.Repository
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    public class ClubRepository : BaseRepository<Club>, IClubRepository
    {
        public ClubRepository(BoardGameAppDbContext context)
            : base(context) 
        {
        }

        public async Task<Club?> GetDetailsByIdAsync(Guid id)
        {
            return await dbSet
                .Include(c => c.City)
                .Include(c => c.ClubBoardGames)
                    .ThenInclude(cb => cb.BoardGame)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }
    }

}

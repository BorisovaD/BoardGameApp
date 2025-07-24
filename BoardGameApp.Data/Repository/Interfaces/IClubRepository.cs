namespace BoardGameApp.Data.Repository.Interfaces
{
    using BoardGameApp.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IClubRepository : IRepository<Club>
    {
        Task<Club?> GetDetailsByIdAsync(Guid id);
    }
}

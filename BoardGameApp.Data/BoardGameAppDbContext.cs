using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BoardGameApp.Data
{
    public class BoardGameAppDbContext : IdentityDbContext
    {
        public BoardGameAppDbContext(DbContextOptions<BoardGameAppDbContext> options)
            : base(options)
        {
        }
    }
}

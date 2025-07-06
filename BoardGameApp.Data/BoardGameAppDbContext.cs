using BoardGameApp.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static BoardGameApp.GCommon.ApplicationConstants;

namespace BoardGameApp.Data
{
    public class BoardGameAppDbContext : IdentityDbContext
    {
        public BoardGameAppDbContext(DbContextOptions<BoardGameAppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BoardgameUser> BoardgameUsers { get; set; } = null!;
        public virtual DbSet<City> Cities { get; set; } = null!;
        public virtual DbSet<Club> Clubs { get; set; } = null!;
        public virtual DbSet<BoardGame> BoardGames { get; set; } = null!;
        public virtual DbSet<ClubBoardGame> ClubBoardGames { get; set; } = null!;
        public virtual DbSet<BoardgameUserFavorite> BoardgameUserFavorites { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<BoardGameCategory> BoardGameCategories { get; set; } = null!;
        public virtual DbSet<GameSession> GameSessions { get; set; } = null!;
        public virtual DbSet<GameRanking> GameRankings { get; set; } = null!;
        public virtual DbSet<Reservation> Reservations { get; set; } = null!;
        public virtual DbSet<Ticket> Tickets { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.Property(t => t.Price)
                      .HasColumnType(PriceType);
            });
        }
    }
}

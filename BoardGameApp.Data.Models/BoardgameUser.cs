namespace BoardGameApp.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BoardgameUser : IdentityUser<Guid>
    {
        public virtual ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();

        public virtual ICollection<BoardgameUserFavorite> FavoriteBoardGames { get; set; } = new HashSet<BoardgameUserFavorite>();

        public virtual ICollection<GameRanking> GameRankings { get; set; } = new HashSet<GameRanking>();

        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    }
}

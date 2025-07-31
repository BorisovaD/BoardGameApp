namespace BoardGameApp.Web.ViewModels.Manager.ClubBoardGames
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ClubBoardGamesCatalogViewModel
    {
        public Guid BoardGameId { get; set; }

        public Guid ClubId { get; set; }

        public string Title { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public int MinPlayers { get; set; }

        public int MaxPlayers { get; set; }

        public int Duration { get; set; } 

        public bool IsInClub { get; set; }
    }
}

namespace BoardGameApp.Web.ViewModels.GameSession
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GameSessionsViewModel
    {
        public Guid Id { get; set; }

        public string BoardGameName { get; set; } = null!;

        public string ClubName { get; set; } = null!; 

        public string CityName { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int MaxPlayers { get; set; }

        public int CurrentPlayers { get; set; }

        public int FreeSlots => MaxPlayers - CurrentPlayers;        
    }
}

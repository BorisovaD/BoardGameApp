namespace BoardGameApp.Web.ViewModels.Manager.Reservations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GameSessionTicketViewModel
    {
        public Guid GameSessionId { get; set; }
        public string GameTitle { get; set; } = null!;
        public int MaxPlayers { get; set; }
        public int CurrentPlayers { get; set; }
        public bool CanEdit => CurrentPlayers == 0;

        public int AvailableTickets => MaxPlayers - CurrentPlayers;
    }
}

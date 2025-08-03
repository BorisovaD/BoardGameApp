namespace BoardGameApp.Web.ViewModels.Ticket
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BuyTicketViewModel
    {
        public Guid GameSessionId { get; set; }

        [Display(Name = "🎟️ Available Tickets")]
        public int AvailableTickets { get; set; }

        [Range(1, 10, ErrorMessage = "You must select between 1 and 10 tickets.")]
        [Display(Name = "Number of Tickets")]
        public int TicketsToBuy { get; set; }

        public string GameTitle { get; set; } = null!;
    }
}

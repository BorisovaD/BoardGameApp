namespace BoardGameApp.Web.ViewModels.Ticket
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TicketDetailsViewModel : TicketIndexViewModel
    {
        public DateTime ReservationTime { get; set; }

        public Guid GameSessionId { get; set; }

        public DateTime StartTime { get; set; }
                
        public DateTime EndTime { get; set; }
    }
}

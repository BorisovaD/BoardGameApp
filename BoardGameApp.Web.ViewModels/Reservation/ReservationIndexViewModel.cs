namespace BoardGameApp.Web.ViewModels.Reservation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ReservationIndexViewModel
    {
        public Guid Id { get; set; }

        public DateTime ReservationTime { get; set; }

        public string UserEmail { get; set; } = null!;

        public Guid GameSessionId { get; set; }

        public DateTime? SessionStartTime { get; set; }

        public Guid BoardGameId { get; set; }

        public string BoardGameTitle { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public int Tickets { get; set; }
    }
}

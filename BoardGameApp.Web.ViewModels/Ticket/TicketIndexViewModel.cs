namespace BoardGameApp.Web.ViewModels.Ticket
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TicketIndexViewModel
    {
        public Guid Id { get; set; }

        public DateTime IssuedOn { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string UserName { get; set; } = null!;

        public Guid ReservationId { get; set; }

        public string BoardGameTitle { get; set; } = null!;
    }
}

namespace BoardGameApp.Data.Models
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    public class Ticket
    {
        [Key]
        [Comment("Unique identifier for the ticket")]
        public Guid Id { get; set; }

        [Comment("Date and time when the ticket was issued")]
        public DateTime IssuedOn { get; set; }
        
        [Comment("Ticket price")]
        public decimal Price { get; set; }

        [Comment("Tickets quantity bought")]
        public int Quantity { get; set; }

        [Required]
        [Comment("Identifier of the user")]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual BoardgameUser User { get; set; } = null!;

        [Required]
        [Comment("Identifier of the user")]
        public Guid ReservationId { get; set; }

        [ForeignKey(nameof(ReservationId))]
        public virtual Reservation Reservation { get; set; } = null!;
    }
}

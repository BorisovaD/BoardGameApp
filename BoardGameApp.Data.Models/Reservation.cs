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

    public class Reservation
    {
        [Key]
        [Comment("Reservation Unique Identifier")]
        public Guid Id { get; set; }

        [Comment("Time when the reservation was made")]
        public DateTime ReservationTime { get; set; }

        [Required]
        [Comment("Identifier of the user")]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual BoardgameUser User { get; set; } = null!;

        [Required]
        [Comment("Identifier of the game session")]
        public Guid GameSessionId { get; set; }

        [ForeignKey(nameof(GameSessionId))]
        public virtual GameSession GameSession { get; set; } = null!;
    }
}

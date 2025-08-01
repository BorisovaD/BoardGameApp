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

    public class GameSession
    {
        [Key]
        [Comment("Game session Unique Identifier")]
        public Guid Id { get; set; }

        [Required]
        [Comment("Game session start")]
        public DateTime StartTime { get; set; }

        [Required]
        [Comment("Game session end")]
        public DateTime EndTime { get; set; }

        [Range(1, 100)]
        [Comment("Maximum number of players for the session")]
        public int MaxPlayers { get; set; }

        [Range(0, 100)]
        [Comment("Number of currently registered players")]
        public int CurrentPlayers { get; set; }

        [Required]
        [Comment("Identifier of the game")]
        public Guid BoardGameId { get; set; }

        [ForeignKey(nameof(BoardGameId))]
        public virtual BoardGame BoardGame { get; set; } = null!;

        [Required]
        [Comment("Identifier of the club")]
        public Guid ClubId { get; set; }

        [ForeignKey(nameof(ClubId))]
        public virtual Club Club { get; set; } = null!;

        [Required]
        [Comment("Identifier of the user who organized the session")]
        public Guid OrganizerId { get; set; }

        [ForeignKey(nameof(OrganizerId))]
        public virtual BoardgameUser Organizer { get; set; } = null!;

        [Comment("Indicates if the game session is deleted")]
        public bool IsDeleted { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
    }
}

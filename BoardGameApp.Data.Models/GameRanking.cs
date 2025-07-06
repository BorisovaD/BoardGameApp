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

    public class GameRanking
    {
        [Key]
        [Comment("Game ranking Unique Identifier")]
        public Guid Id { get; set; }

        [Range(0, 5000)]
        [Comment("Player wins")]
        public int Wins { get; set; }

        [Range(0, 5000)]
        [Comment("Player losses")]
        public int Losses { get; set; }

        [Range(0, 5000)]
        [Comment("Player draws")]
        public int Draws { get; set; }

        public DateTime LastUpdated { get; set; }

        [Required]
        [Comment("Identifier of the user")]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual BoardgameUser User { get; set; } = null!;

        [Required]
        [Comment("Identifier of the game")]
        public Guid BoardGameId { get; set; }

        [ForeignKey(nameof(BoardGameId))]
        public virtual BoardGame BoardGame { get; set; } = null!;
    }
}

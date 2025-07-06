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

    [PrimaryKey(nameof(ClubId), nameof(BoardGameId))]
    public class ClubBoardGame
    {        
        [Required]
        [Comment("Identifier of the game club")]
        public Guid ClubId { get; set; }

        [ForeignKey(nameof(ClubId))]
        public virtual Club Club { get; set; } = null!;

        [Required]
        [Comment("Identifier of the game played in this club")]
        public Guid BoardGameId { get; set; }

        [ForeignKey(nameof(BoardGameId))]
        public virtual BoardGame BoardGame { get; set; } = null!;

        [Comment("Indicates if the game is actively offered in this club")]
        public bool IsDeleted { get; set; }
    }
}

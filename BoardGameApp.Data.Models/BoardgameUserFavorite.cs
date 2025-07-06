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
    using static BoardGameApp.Data.Common.EntityConstants;

    [PrimaryKey(nameof(UserId), nameof(BoardGameId))]
    public class BoardgameUserFavorite
    {
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

        [Comment("Indicates if the game is actively in user favorite list")]
        public bool IsDeleted { get; set; }
    }
}

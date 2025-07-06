namespace BoardGameApp.Data.Models
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [PrimaryKey(nameof(CategoryId), nameof(BoardGameId))]
    public class BoardGameCategory
    {
        [Required]
        [Comment("Identifier of the game category")]
        public Guid CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;

        [Required]
        [Comment("Identifier of the game in this category")]
        public Guid BoardGameId { get; set; }

        [ForeignKey(nameof(BoardGameId))]
        public virtual BoardGame BoardGame { get; set; } = null!;

        [Comment("Indicates if the game is actively offered in this category")]
        public bool IsDeleted { get; set; }
    }
}

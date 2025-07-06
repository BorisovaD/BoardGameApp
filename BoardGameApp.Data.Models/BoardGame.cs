namespace BoardGameApp.Data.Models
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static BoardGameApp.Data.Common.EntityConstants.BoardGame;
    public class BoardGame
    {
        [Key]
        [Comment("Game Unique Identifier")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(TitleMaxLength)]
        [Comment("Game title")]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(DescriptionMaxLength)]
        [Comment("Game description")]
        public string Description { get; set; } = null!;

        [Comment("The Url of the image")]
        public string? ImageUrl { get; set; }

        [Comment("The Url of the game rules")]
        public string? RulesUrl { get; set; }

        [Range(1, 100)]
        [Comment("Min players needed")]
        public int MinPlayers { get; set; }

        [Range(1, 100)]
        [Comment("Max number of the players")]
        public int MaxPlayers { get; set; }

        [Range(1, 1000)]
        [Comment("Game duration")]
        public int Duration { get; set; }

        [Comment("Shows if game is deleted")]
        public bool IsDeleted { get; set; }

        public virtual ICollection<GameSession> GameSessions { get; set; } = new HashSet<GameSession>();

        public virtual ICollection<GameRanking> Rankings { get; set; } = new 
          HashSet<GameRanking>();

        public virtual ICollection<BoardGameCategory> BoardGameCategories { get; set; } = new HashSet<BoardGameCategory>();

        public virtual ICollection<ClubBoardGame> ClubBoardGames { get; set; } = new HashSet<ClubBoardGame>();

        public virtual ICollection<BoardgameUserFavorite> FavoritedByUsers { get; set; } = new HashSet<BoardgameUserFavorite>();
    }
}

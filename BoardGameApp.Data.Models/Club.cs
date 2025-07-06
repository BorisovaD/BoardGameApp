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
    using static BoardGameApp.Data.Common.EntityConstants.Club;
    public class Club
    {
        [Key]
        [Comment("Club Unique Identifier")]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        [Comment("Club name")]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(AddressMaxLength)]
        [Comment("Location")]
        public string Address { get; set; } = null!;

        [Required]
        [Comment("Identifier of city")]
        public Guid CityId { get; set; }

        [ForeignKey(nameof(CityId))]
        public virtual City City { get; set; } = null!;

        [Comment("Shows if club is deleted")]
        public bool IsDeleted { get; set; }

        public virtual ICollection<GameSession> GameSessions { get; set; } = new HashSet<GameSession>();

        public virtual ICollection<ClubBoardGame> ClubBoardGames { get; set; } = new HashSet<ClubBoardGame>();
    }
}

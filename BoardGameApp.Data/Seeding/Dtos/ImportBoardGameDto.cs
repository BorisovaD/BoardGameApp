namespace BoardGameApp.Data.Seeding.Dtos
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static BoardGameApp.Data.Common.EntityConstants.BoardGame;
    public class ImportBoardGameDto
    {
        [JsonProperty("Id")]
        public Guid Id { get; set; }

        [JsonProperty("Title")]
        [Required]
        [MinLength(TitleMinLength)]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [JsonProperty("Description")]
        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [JsonProperty("ImageUrl")]
        [Url(ErrorMessage = "Invalid ImageUrl format.")]
        public string? ImageUrl { get; set; }

        [JsonProperty("RulesUrl")]
        [Url(ErrorMessage = "Invalid RulesUrl format.")]
        public string? RulesUrl { get; set; }

        [JsonProperty("MinPlayers")]
        [Range(1, 100)]
        public int MinPlayers { get; set; }

        [JsonProperty("MaxPlayers")]
        [Range(1, 100)]
        public int MaxPlayers { get; set; }

        [JsonProperty("Duration")]
        [Range(1, 1000)]
        public int Duration { get; set; }

        [JsonProperty("IsDeleted")]
        public bool IsDeleted { get; set; }
    }
}

namespace BoardGameApp.Data.Seeding.Dtos
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ImportClubBoardGameDto
    {
        [JsonProperty("ClubId")]
        public Guid ClubId { get; set; }

        [JsonProperty("BoardGameId")]
        public Guid BoardGameId { get; set; }

        [JsonProperty("IsDeleted")]
        public bool IsDeleted { get; set; }
    }
}

namespace BoardGameApp.Data.Seeding.Dtos
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static BoardGameApp.Data.Common.EntityConstants.Club;
    public class ImportClubDto
    {
        [JsonProperty("Id")]
        public Guid Id { get; set; }

        [JsonProperty("Name")]
        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [JsonProperty("Address")]
        [Required]
        [MinLength(AddressMinLength)]
        [MaxLength(AddressMaxLength)]
        public string Address { get; set; } = null!;

        [JsonProperty("CityId")]
        public Guid CityId { get; set; }

        [JsonProperty("IsDeleted")]
        public bool IsDeleted { get; set; }
    }
}

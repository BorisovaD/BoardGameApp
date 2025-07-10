namespace BoardGameApp.Data.Seeding.Dtos
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static BoardGameApp.Data.Common.EntityConstants.Category;
    public class ImportCategoryDto
    {
        [JsonProperty("Id")]
        public Guid Id { get; set; }

        [JsonProperty("Name")]
        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [JsonProperty("IsDeleted")]
        public bool IsDeleted { get; set; }
    }
}

namespace BoardGameApp.Data.Models
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static BoardGameApp.Data.Common.EntityConstants.City;
    public class City
    {
        [Key]
        [Comment("City Unique Identifier")]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        [Comment("City name")]
        public string Name { get; set; } = null!;

        [Comment("Shows if city is deleted")]
        public bool IsDeleted { get; set; }

        public virtual ICollection<Club> Clubs { get; set; } = new HashSet<Club>();
    }
}

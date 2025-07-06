namespace BoardGameApp.Data.Models
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static BoardGameApp.Data.Common.EntityConstants.Category;
    public class Category
    {
        [Key]
        [Comment("Category Unique Identifier")]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        [Comment("Category name")]
        public string Name { get; set; } = null!;

        [Comment("Shows if category is deleted")]
        public bool IsDeleted { get; set; }

        public virtual ICollection<BoardGameCategory> BoardGameCategories { get; set; } = new HashSet<BoardGameCategory>();
    }
}

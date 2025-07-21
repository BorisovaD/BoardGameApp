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

    public class Manager
    {
        [Key]
        [Comment("Manager identifier")]
        public Guid Id { get; set; }        

        [Required]
        [Comment("Manager's user entity")]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual BoardgameUser User { get; set; } = null!;

        [Required]
        [Comment("The club this manager is responsible for")]
        public Guid ClubId { get; set; }

        [ForeignKey(nameof(ClubId))]
        public virtual Club Club { get; set; } = null!;

        [Comment("Soft delete flag")]
        public bool IsDeleted { get; set; }
    }
}

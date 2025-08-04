namespace BoardGameApp.Web.ViewModels.Manager.GameSessions
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GameSessionEditModel
    {
        public Guid Id { get; set; } 

        [Required]
        [Display(Name = "Start Time (hour)")]
        [Range(0, 23)]
        public int StartTime { get; set; }

        [Required]
        [Display(Name = "End Time (hour)")]
        [Range(1, 23)]
        public int EndTime { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Max Players")]
        public int MaxPlayers { get; set; }

        [Required]
        [Display(Name = "Board Game")]
        public Guid BoardGameId { get; set; }

        [Required]
        [Display(Name = "Club")]
        public Guid ClubId { get; set; }

        public IEnumerable<SelectListItem> BoardGames { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> Clubs { get; set; } = new List<SelectListItem>();
    }
}

namespace BoardGameApp.Web.ViewModels.Manager.GameSessions
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AddGameSessionViewModel
    {
        [Required]
        [Display(Name = "Start Hour")]
        [Range(0, 23)]
        public int StartHour { get; set; }

        [Required]
        [Display(Name = "End Hour")]
        [Range(0, 23)]
        public int EndHour { get; set; }

        [Required]
        [Display(Name = "Max Players")]
        [Range(1, 100)]
        public int MaxPlayers { get; set; }

        [Required]
        [Display(Name = "Board Game")]
        public Guid BoardGameId { get; set; }

        [Required]
        [Display(Name = "Club")]
        public Guid ClubId { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        public IEnumerable<SelectListItem> AvailableBoardGames { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> AvailableClubs { get; set; } = new List<SelectListItem>();
    }
}

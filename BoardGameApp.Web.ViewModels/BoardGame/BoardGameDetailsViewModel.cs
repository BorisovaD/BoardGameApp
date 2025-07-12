namespace BoardGameApp.Web.ViewModels.BoardGame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BoardGameDetailsViewModel
    {
        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public string? RulesUrl { get; set; }

        public string MinPlayers { get; set; } = null!;

        public string MaxPlayers { get; set; } = null!;

        public string Duration { get; set; } = null!;

        public bool IsSaved { get; set; }

        public List<string> Categories { get; set; } = new List<string>();
    }
}

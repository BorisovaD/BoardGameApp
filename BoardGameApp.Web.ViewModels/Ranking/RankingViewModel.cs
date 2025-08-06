namespace BoardGameApp.Web.ViewModels.Ranking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RankingViewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = null!;

        public string BoardGameTitle { get; set; } = null!;

        public int Wins { get; set; }

        public int Losses { get; set; }

        public int Draws { get; set; }        

        public string? Medal { get; set; }
    }
}

namespace BoardGameApp.Web.ViewModels.Manager.GameRanking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GameRankingBaseModel
    {
        public Guid Id { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
    }
}

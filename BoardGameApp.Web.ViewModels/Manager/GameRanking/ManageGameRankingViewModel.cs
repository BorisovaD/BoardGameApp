namespace BoardGameApp.Web.ViewModels.Manager.GameRanking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ManageGameRankingViewModel : GameRankingBaseModel
    {        
        public string Username { get; set; } = null!;

        public string BoardGameTitle { get; set; } = null!;
               
        public DateTime? LastUpdated { get; set; }
    }
}

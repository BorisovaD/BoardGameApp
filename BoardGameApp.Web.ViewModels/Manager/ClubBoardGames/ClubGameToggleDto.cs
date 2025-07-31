namespace BoardGameApp.Web.ViewModels.Manager.ClubBoardGames
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ClubGameToggleDto
    {
        public Guid ClubId { get; set; }
        public Guid GameId { get; set; }
    }
}

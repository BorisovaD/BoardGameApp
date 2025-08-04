namespace BoardGameApp.Web.ViewModels.Manager.GameSessions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GameSessionInputModel
    {
        public Guid BoardGameId { get; set; }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

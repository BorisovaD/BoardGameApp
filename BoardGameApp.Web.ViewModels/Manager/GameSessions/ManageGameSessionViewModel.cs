namespace BoardGameApp.Web.ViewModels.Manager.GameSessions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ManageGameSessionViewModel
    {
        public Guid Id { get; set; }

        public Guid? GameSessionId { get; set; }

        public Guid ClubId { get; set; }

        public Guid BoardGameId { get; set; }

        public string BoardGameTitle { get; set; } = null!;

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}

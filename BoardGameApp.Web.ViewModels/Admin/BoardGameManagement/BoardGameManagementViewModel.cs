namespace BoardGameApp.Web.ViewModels.Admin.BoardGameManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public  class BoardGameManagementViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;       
        public int Duration { get; set; }
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }

        public IEnumerable<string> Categories { get; set; } = new List<string>();
        public bool IsDeleted { get; set; }
    }
}

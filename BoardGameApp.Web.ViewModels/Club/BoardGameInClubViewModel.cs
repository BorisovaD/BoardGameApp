namespace BoardGameApp.Web.ViewModels.Club
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BoardGameInClubViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;

        public Guid? ActiveGameSessionId { get; set; }
    }
}

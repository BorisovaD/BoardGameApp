namespace BoardGameApp.Web.ViewModels.Club
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ClubDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string CityName { get; set; } = null!;
        public IEnumerable<BoardGameInClubViewModel> BoardGames { get; set; } = new List<BoardGameInClubViewModel>();
    }
}

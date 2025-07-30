namespace BoardGameApp.Web.ViewModels.Admin.BoardGameManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DeleteBoardGameInputModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string? ImageUrl { get; set; }
    }
}

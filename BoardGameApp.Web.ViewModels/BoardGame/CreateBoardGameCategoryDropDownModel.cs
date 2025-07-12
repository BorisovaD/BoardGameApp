namespace BoardGameApp.Web.ViewModels.BoardGame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CreateBoardGameCategoryDropDownModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
    }
}

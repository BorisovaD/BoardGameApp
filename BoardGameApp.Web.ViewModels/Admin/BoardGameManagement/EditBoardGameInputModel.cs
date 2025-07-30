namespace BoardGameApp.Web.ViewModels.Admin.BoardGameManagement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EditBoardGameInputModel : AddBoardGameInputModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}

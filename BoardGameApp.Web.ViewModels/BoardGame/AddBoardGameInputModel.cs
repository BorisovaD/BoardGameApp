namespace BoardGameApp.Web.ViewModels.BoardGame
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static BoardGameApp.Data.Common.EntityConstants.BoardGame;

    public class AddBoardGameInputModel
    {
        [Required]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;
                
        public string? ImageUrl { get; set; }

        public string? RulesUrl { get; set; }

        [Range(1, 100)]
        public int MinPlayers { get; set; }

        [Range(1, 100)]
        public int MaxPlayers { get; set; }

        [Range(1, 1000)]
        public int Duration { get; set; }

        public IEnumerable<CreateBoardGameCategoryDropDownModel>? Categories { get; set; }

        [Display(Name = "Select categories")]
        [Required]
        public List<Guid> SelectedCategoryIds { get; set; } = new();
    }
}

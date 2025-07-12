namespace BoardGameApp.Services.Core
{
    using BoardGameApp.Data;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.BoardGame;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CategoryService : ICategoryService
    {
        private readonly BoardGameAppDbContext dbContext;
        public CategoryService(BoardGameAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<CreateBoardGameCategoryDropDownModel>> GetCategoriesDropDownDataAsync()
        {
            IEnumerable<CreateBoardGameCategoryDropDownModel> categoriesAsDropDown = await this.dbContext
            .Categories
            .AsNoTracking()
            .Select(c => new CreateBoardGameCategoryDropDownModel()
            {
                Id = c.Id,
                Name = c.Name,
            })
            .ToArrayAsync();

            return categoriesAsDropDown;
        }
    }
}

namespace BoardGameApp.Services.Core
{
    using BoardGameApp.Data;
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
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
        private readonly IRepository<Category> categoryRepository;

        public CategoryService(IRepository<Category> categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public async Task<IEnumerable<CreateBoardGameCategoryDropDownModel>> GetCategoriesDropDownDataAsync()
        {
            IEnumerable<CreateBoardGameCategoryDropDownModel> categoriesAsDropDown = await categoryRepository
            .All()
            .Where(c => c.IsDeleted == false)
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

namespace BoardGameApp.Services.Core.Tests.Admin
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Admin;
    using Moq;
    using MockQueryable;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestFixture]
    public class CategoryServiceTests
    {
        private Mock<IRepository<Category>> categoryRepositoryMock;
        private CategoryService categoryService;

        [SetUp]
        public void Setup()
        {
            categoryRepositoryMock = new Mock<IRepository<Category>>(MockBehavior.Strict);
            categoryService = new CategoryService(categoryRepositoryMock.Object);
        }

        [Test]
        public void PassAlways()
        {
            Assert.Pass();
        }

        [Test]
        public async Task GetCategoriesDropDownDataAsync_ShouldReturnOnlyNonDeletedCategories()
        {
            
            var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Strategy", IsDeleted = false },
            new Category { Id = Guid.NewGuid(), Name = "Family", IsDeleted = true },
            new Category { Id = Guid.NewGuid(), Name = "Fun", IsDeleted = false },
        };

            var categoriesQueryable = categories.BuildMock();

            categoryRepositoryMock
                .Setup(r => r.All())
                .Returns(categoriesQueryable);
                        
            var result = await categoryService.GetCategoriesDropDownDataAsync();
                        
            Assert.IsNotNull(result);
            var list = result.ToList();
            Assert.That(list.Count, Is.EqualTo(2)); 
            Assert.IsTrue(list.Any(c => c.Name == "Strategy"));
            Assert.IsTrue(list.Any(c => c.Name == "Fun"));
            Assert.IsFalse(list.Any(c => c.Name == "Family"));
        }

        [Test]
        public async Task GetCategoriesDropDownDataAsync_ShouldReturnEmptyList_WhenNoCategories()
        {            
            var emptyList = new List<Category>().BuildMock();

            categoryRepositoryMock
                .Setup(r => r.All())
                .Returns(emptyList);
                        
            var result = await categoryService.GetCategoriesDropDownDataAsync();
                        
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }
    }
}

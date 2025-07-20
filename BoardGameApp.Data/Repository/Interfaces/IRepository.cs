namespace BoardGameApp.Data.Repository.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> GetAllAsync();

        IQueryable<T> All();

        Task AddAsync(T entity);

        void Update(T entity);

        Task ReturnExisting(T entity);

        public void Delete(T entity);

        Task SoftDeleteAsync(T entity);

        Task<bool> AnyAsync(Guid id);

        Task SaveChangesAsync();
    }
}

namespace BoardGameApp.Data.Repository
{
    using BoardGameApp.Data.Repository.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly BoardGameAppDbContext dbContext;
        protected readonly DbSet<T> dbSet;

        public BaseRepository(BoardGameAppDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public IQueryable<T> All()  
        {
            return dbContext.Set<T>();
        }

        public async Task<bool> AnyAsync(Guid id)
        {
            var entity = await dbSet.FindAsync(id);

            return entity != null;
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet
            .Where(e => EF.Property<bool>(e, "IsDeleted") == false)
            .ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task ReturnExisting(T entity)
        {
            var entry = dbContext.Entry(entity);
            if (entry.Properties.Any(p => p.Metadata.Name == "IsDeleted"))
            {
                entry.Property("IsDeleted").CurrentValue = false;
                dbSet.Update(entity);
            }

            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(T entity)
        {
            var entry = dbContext.Entry(entity);

            if (entry.Properties.Any(p => p.Metadata.Name == "IsDeleted"))
            {
                entry.Property("IsDeleted").CurrentValue = true;

                dbSet.Update(entity);
            }

            await Task.CompletedTask;
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }
    }
}

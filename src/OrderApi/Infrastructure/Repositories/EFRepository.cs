using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OrderApi.Infrastructure.Repositories
{
    public abstract class EFRepository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext Database;

        public EFRepository(DbContext context)
        {
            Database = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Database.Set<T>().ToListAsync();
        }

        public async Task<T> GetByAsync(int id)
        {
            return await Database.Set<T>().FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            await Database.AddAsync(entity);
            await Database.SaveChangesAsync();

            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            Database.Update(entity);
            await Database.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            Database.Remove(entity);
            await Database.SaveChangesAsync();

            return true;
        }
    }
}
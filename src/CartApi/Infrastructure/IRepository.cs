using System.Threading.Tasks;

namespace CartApi.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByAsync(string key);
        Task<T> UpdateAsync(string key, T entity);
        Task<bool> DeleteAsync(string key);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityApi.Data.Repos {
    public interface IRepository<T> where T : class {
        Task<IEnumerable<T>> GetAll ();
        Task<T> GetBy (int id);
        Task<T> Add (T entity);
        Task Update (T entity);
        Task Delete (T entity);
    }
}
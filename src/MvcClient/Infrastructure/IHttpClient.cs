using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcClient.Infrastructure
{
    public interface IHttpClient
    {
        Task<IEnumerable<T>> GetListAsync<T>(string uri) where T : class;
        Task<T> GetAsync<T>(string uri) where T : class;
        Task<T> PostAsync<T>(string uri, object entity);
        Task<T> PutAsync<T>(string uri, object entity);
        Task DeleteAsync(string uri);
    }
}
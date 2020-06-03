using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CartApi.Infrastructure
{
    public abstract class RedisRepository<T> : IRepository<T> where T : class
    {
        protected readonly IDatabase database;

        public RedisRepository(IDatabase database)
        {
            this.database = database;
        }
        public async Task<T> GetByAsync(string key)
        {
            var data = await database.StringGetAsync(key);

            return data.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<T>(data);
        }

        public async Task<T> UpdateAsync(string key, T entity)
        {
            var data = JsonConvert.SerializeObject(entity);
            var created = await database.StringSetAsync(key, data);
            
            return !created ? null : await GetByAsync(key);
        }

        public async Task<bool> DeleteAsync(string key)
        {
            return await database.KeyDeleteAsync(key);
        }
    }
}
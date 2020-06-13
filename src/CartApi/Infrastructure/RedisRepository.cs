using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
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
            TimeSpan time = new TimeSpan(0, 30, 0);
            await database.KeyExpireAsync(key, time);
            return !created ? null : await GetByAsync(key);
        }


        public async Task<bool> DeleteAsync(string key)
        {

            // database.KeyExpireAsync(RedisChannel,)
            return await database.KeyDeleteAsync(key);
        }
    }
}
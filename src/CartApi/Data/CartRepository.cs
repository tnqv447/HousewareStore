using CartApi.Infrastructure;
using CartApi.Models;
using StackExchange.Redis;

namespace CartApi.Data
{
    public class CartRepository : RedisRepository<Cart>, ICartRepository
    {
        public CartRepository(ConnectionMultiplexer redis) : base(redis.GetDatabase())
        {
        }
    }
}
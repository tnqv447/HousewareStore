using System.Threading.Tasks;
using CartApi.Infrastructure;

namespace CartApi.Models
{
    public interface ICartRepository : IRepository<Cart>
    {
    }
}
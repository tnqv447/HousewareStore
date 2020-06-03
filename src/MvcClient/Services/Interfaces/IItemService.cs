using System.Threading.Tasks;
using MvcClient.ViewModels;

using MvcClient.Models;

namespace MvcClient.Services
{
    public interface IItemService
    {
        Task<IndexViewModel> GetCatalog(string category, string searchString1);
        Task<Item> GetItem(int id);
        Task CreateItem(Item item);
        Task UpdateItem(int id, Item item);
        Task DeleteItem(int id);
        Task ChangeItemStatus(int id, ItemStatus status);

    }
}
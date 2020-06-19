using System.Collections.Generic;
using System.Threading.Tasks;
using MvcClient.ViewModels;

using MvcClient.Models;

namespace MvcClient.Services
{
    public interface IItemService
    {
        Task<IndexViewModel> GetCatalog(string category = null, string searchString = null, double minPrice = 0,
                                        double maxPrice = 0, string sortOrder = null, bool isAdmin = false, string userId = null);
        Task<IEnumerable<Item>> GetItemsSale(string saleId);
        Task<IList<Category>> GetCategories();
        Task<Item> GetItem(int id);
        Task<Item> CreateItem(Item item);
        Task UpdateItem(int id, Item item);
        Task DeleteItem(int id);
        Task ChangeItemStatus(int id, ItemStatus status);
        Task<IEnumerable<Item>> GetAll();

    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ItemApi.DTOs;
using ItemApi.Models;

namespace ItemApi.Data.Repos
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<IEnumerable<ItemDTO>> GetItemsBySearch(string category = null, string searchString = null, double minPrice = 0, double maxPrice = 0,
                                            string sortOrder = null, bool isAdmin = false, string userId = null, DbStatus dbStatus = DbStatus.All);
        //Task<IEnumerable<ItemDTO>> GetItemsBySearch (List<Item> items, string category = null, string searchString = null);
        bool ItemExists(int id);
        Task Activate(int id);
        Task Disable(int id);

        Task<IEnumerable<ItemDTO>> MappingToItemDTO(IQueryable<Item> items);
        Task<ItemDTO> MappingToItemDTO(Item item);


    }
}
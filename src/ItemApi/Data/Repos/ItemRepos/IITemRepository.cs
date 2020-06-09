using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ItemApi.DTOs;
using ItemApi.Models;

<<<<<<< HEAD
namespace ItemApi.Data.Repos {
    public interface IItemRepository : IRepository<Item> {
        Task<IEnumerable<ItemDTO>> GetItemsBySearch (string category = null, string searchString = null,string sortOrder = " ", DbStatus dbStatus = DbStatus.All);
=======
namespace ItemApi.Data.Repos
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<IEnumerable<ItemDTO>> GetItemsBySearch(string category = null, string searchString = null, DbStatus dbStatus = DbStatus.All);
>>>>>>> 2065de25e9aae98f39adc274249eeaa7326a4ab0
        //Task<IEnumerable<ItemDTO>> GetItemsBySearch (List<Item> items, string category = null, string searchString = null);
        bool ItemExists(int id);
        Task Activate(int id);
        Task Disable(int id);

        Task<IEnumerable<ItemDTO>> MappingToItemDTO(IQueryable<Item> items);
        Task<ItemDTO> MappingToItemDTO(Item item);


    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using ItemApi.DTOs;
using ItemApi.Models;

namespace ItemApi.Data.Repos {
    public interface IItemRepository : IRepository<Item> {
        Task<IEnumerable<ItemDTO>> GetItemsBySearch (string category = null, string searchString = null,string sortOrder = " ", DbStatus dbStatus = DbStatus.All);
        //Task<IEnumerable<ItemDTO>> GetItemsBySearch (List<Item> items, string category = null, string searchString = null);
        bool ItemExists (int id);
        Task Activate (int id);
        Task Disable (int id);

    }
}
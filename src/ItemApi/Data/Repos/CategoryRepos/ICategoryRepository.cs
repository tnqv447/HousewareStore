using System.Collections.Generic;
using System.Threading.Tasks;
using ItemApi.Models;
using ItemApi.DTOs;


namespace ItemApi.Data.Repos
{
    public interface ICategoryRepository : IRepository<Category>
    {
        int GetCategoryIdByName(string categoryName);
        Task<IEnumerable<string>> GetAllCategoryNames();
        Task<IEnumerable<int>> GetAllCategoryIds();
        Task<IEnumerable<Category>> GetAllCategories();

    }
}
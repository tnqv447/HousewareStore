using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

using ItemApi.DTOs;
using ItemApi.Data;
using ItemApi.Models;

namespace ItemApi.Data.Repos
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ItemContext _context;
        private readonly IMapper _mapper;

        public CategoryRepository(ItemContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public int GetCategoryIdByName(string categoryName)
        {
            if (!string.IsNullOrEmpty(categoryName))
            {
                var categories = _context.Categories.AsQueryable().Where(m => m.CategoryName.Equals(categoryName)).Select(m => m.CategoryId).ToList();
                if (categories.Count() == 1)
                {
                    return categories.First();
                }
            }
            return -1;
        }

        public async Task<IEnumerable<string>> GetAllCategoryNames()
        {
            return await _context.Categories
                .OrderBy(m => m.CategoryId)
                .Select(m => m.CategoryName)
                .Distinct().ToListAsync();
        }

        public async Task<IEnumerable<int>> GetAllCategoryIds()
        {
            return await _context.Categories
                .OrderBy(m => m.CategoryId)
                .Select(m => m.CategoryId)
                .Distinct().ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ItemApi.DTOs;
using ItemApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ItemApi.Data.Repos
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        private readonly ItemContext _context;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepos;

        public ItemRepository(ItemContext context, IMapper mapper, ICategoryRepository categoryRepos) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _categoryRepos = categoryRepos;
        }

        public async Task<IEnumerable<ItemDTO>> GetItemsBySearch(string category = null, string searchString = null, DbStatus dbStatus = DbStatus.Active)
        {

            var Items = dbStatus == DbStatus.All ? _context.Items : _context.Items.Where(m => m.DbStatus.Equals(dbStatus));

            if (!string.IsNullOrEmpty(category))
            {
                var categoryId = _categoryRepos.GetCategoryIdByName(category);
                Items = Items.Where(m => m.CategoryId == categoryId);
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                Items = Items.Where(m => m.Name.Contains(searchString));
            }

            return await Items
                .Select(m => _mapper.Map<ItemDTO>(m)).ToListAsync();
        }

        public bool ItemExists(int id)
        {
            return _context.Items.Any(m => m.Id == id);
        }

        public async Task Activate(int id)
        {
            await this.ChangeDbStatus(id, DbStatus.Active);
        }

        public async Task Disable(int id)
        {
            await this.ChangeDbStatus(id, DbStatus.Disabled);
        }

        private async Task ChangeDbStatus(int id, DbStatus dbStatus)
        {
            var item = await this.GetBy(id);
            if (item != null || item.DbStatus != dbStatus)
            {
                item.DbStatus = dbStatus;
                await this.Update(item);
            }
        }

    }
}
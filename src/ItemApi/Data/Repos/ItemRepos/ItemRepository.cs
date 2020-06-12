using System;
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

        public async Task<IEnumerable<ItemDTO>> GetItemsBySearch(string category = null, string searchString = null, double minPrice = 0, double maxPrice = 0, string sortOrder = null, bool isAdmin = false, DbStatus dbStatus = DbStatus.Active)
        {

            var Items = dbStatus == DbStatus.All ? _context.Items : _context.Items.Where(m => m.DbStatus.Equals(dbStatus));
            if (!isAdmin)
            {
                Items = Items.Where(m => m.ItemStatus == ItemStatus.Approved);
            }
            if (!string.IsNullOrEmpty(category))
            {
                var categoryId = _categoryRepos.GetCategoryIdByName(category);
                Items = Items.Where(m => m.CategoryId == categoryId);
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                Items = Items.Where(m => m.Name.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(sortOrder))
            {
                switch (sortOrder)
                {
                    case "name_desc":

                        Items = Items.OrderByDescending(m => m.Name.ToLower());
                        break;
                    case "price":
                        Items = Items.OrderBy(m => m.UnitPrice);
                        break;
                    case "price_desc":
                        Items = Items.OrderByDescending(m => m.UnitPrice);
                        break;
                    default:
                        Items = Items.OrderBy(m => m.Name.ToLower());
                        break;
                }

            }
            if (minPrice >= 0 && maxPrice != 0 && maxPrice >= minPrice)
            {
                Items = Items.Where(m => m.UnitPrice >= minPrice && m.UnitPrice <= maxPrice);
            }
            // return await Items.Select(m => _mapper.Map<ItemDTO>(m)).ToListAsync(); 
            return await MappingToItemDTO(Items);
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

        public async Task<IEnumerable<ItemDTO>> MappingToItemDTO(IQueryable<Item> items)
        {
            var ItemsDto = from i in items
                           join c in _context.Categories on i.CategoryId equals c.CategoryId into catGroup
                           from cat in catGroup
                           select new ItemDTO(i, cat.CategoryName);
            return await ItemsDto.ToListAsync();
        }
        public async Task<ItemDTO> MappingToItemDTO(Item item)
        {
            var Category = await _categoryRepos.GetBy(item.CategoryId);
            string CategoryName = Category.CategoryName;
            var ItemsDto = new ItemDTO(item, CategoryName);
            return ItemsDto;
        }

    }
}
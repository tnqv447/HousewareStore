using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ItemApi.DTOs;
using ItemApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<IEnumerable<ItemDTO>> GetItemsBySearch(string category = null, string searchString = null, double minPrice = 0, double maxPrice = 0, string sortOrder = null, DbStatus dbStatus = DbStatus.Active)
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
            if (!string.IsNullOrEmpty(sortOrder))
            {
                //bà mẹ m, dotnet run làm nãy giờ sửa có chạy cái đéo gì đâu
                switch (sortOrder)
                {
                    case "name_desc":
                        Items = Items.OrderByDescending(m => m.Name);
                        break;
                    case "price":
                        Items = Items.OrderBy(m => m.UnitPrice);
                        break;
                    case "price_desc":
                        Items = Items.OrderByDescending(m => m.UnitPrice);
                        break;
                    default:
                        Items = Items.OrderBy(m => m.Name);
                        break;
                }



            }
            if (minPrice != 0 && maxPrice != 0)
            {
                Items = Items.Where(m => m.UnitPrice >= minPrice && m.UnitPrice <= maxPrice);
            }
            // where gì, t mapper mà vs lại list items vẫn là nó chứ có thay đổi đâu, m ngáo r
            //đây là sql sai
            // return await Items.Select(m => _mapper.Map<ItemDTO>(m)).ToListAsync(); // cơ mà cái return này thì t mới sửa hồi nãy, hàm thì tạo hôm qua nhưng ko dùng cho hàm search này
            return await MappingToItemDTO(Items);// nhiệm vụ cái này là mapper item to itemdto // itemdto có chư cả caterogyName và id
        }// cái này t thêm từ hôn qua rồi // tại cái trên thì ko trả về caterogyName, ủa mà hqua m chỉnh r, nãy t mới teesst bên t vẫn ok, m có chỉnh chỗ nào nữa k v

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
                           from cat in catGroup.DefaultIfEmpty()
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
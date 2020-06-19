using System;
using System.Threading.Tasks;
using AutoMapper;
using ItemApi.Data.Repos;
using ItemApi.DTOs;
using ItemApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
namespace ItemApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemRepository _itemRepos;
        private readonly ICategoryRepository _categoryRepos;
        private readonly IMapper _mapper;

        public ItemController(IItemRepository itemRepos, ICategoryRepository categoryRepos, IMapper mapper)
        {
            _itemRepos = itemRepos;
            _categoryRepos = categoryRepos;
            _mapper = mapper;
        }

        // GET /catalog
        [AllowAnonymous]
        [HttpGet("catalog")]
        public async Task<IndexDTO> GetCatalog(string category = null, string searchString = null, double minPrice = 0, double maxPrice = 0,
                                    string sortOrder = null, bool isAdmin = false, string userId = null, DbStatus status = DbStatus.Active)
        {
            var indexDTO = new IndexDTO()
            {
                ItemCategory = category,
                SearchString = searchString,
                Categories = await _categoryRepos.GetAllCategoryNames(),
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortOrder = sortOrder,

                CategoriesId = await _categoryRepos.GetAllCategoryIds(),
                Items = await _itemRepos.GetItemsBySearch(category, searchString, minPrice, maxPrice, sortOrder, isAdmin, userId, status)
            };

            return indexDTO;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDTO>> GetItem(int id)
        {
            var Item = await _itemRepos.GetBy(id);

            if (Item == null)
            {
                return NotFound();
            }
            // return _mapper.Map<ItemDTO>(Item);//// cái vừa sửa chỉ có ở hàm search đó thui
            return await _itemRepos.MappingToItemDTO(Item);
        }

        [AllowAnonymous]
        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryDTO>>> GetCategories()
        {
            var categories = await _categoryRepos.GetAll();
            var result = new List<CategoryDTO>();
            foreach (var cate in categories)
            {
                result.Add(_mapper.Map<CategoryDTO>(cate));
            }
            return result;
        }
        [AllowAnonymous]
        [HttpGet("sale")]
        public async Task<IEnumerable<ItemDTO>> getItemsSale(string saleId)
        {
            var result = await _itemRepos.GetItemsSale(saleId);
            return result;
        }
        [HttpPost]
        public async Task<ActionResult<Item>> Create(ItemDTO ItemDTO)
        {
            var Item = _mapper.Map<Item>(ItemDTO);

            await _itemRepos.Add(Item);

            return CreatedAtAction(nameof(GetItem), new { id = Item.Id }, Item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ItemDTO ItemDTO)
        {
            if (id != ItemDTO.Id)
            {
                return BadRequest();
            }

            // var Item = await _itemRepos.GetBy(id);
            Item Item = new Item(ItemDTO);
            if (Item == null)
            {
                return NotFound();
            }

            // _mapper.Map<Item>(ItemDTO);

            try
            {
                await _itemRepos.Update(Item);
            }
            catch (DbUpdateConcurrencyException) when (!_itemRepos.ItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var Item = await _itemRepos.GetBy(id);

            if (Item == null)
            {
                return NotFound();
            }

            await _itemRepos.Delete(Item);

            return NoContent();
        }

        [HttpGet("all")]
        public async Task<IEnumerable<ItemDTO>> getAll()
        {
            return await _itemRepos.getAll();
        }
    }
}
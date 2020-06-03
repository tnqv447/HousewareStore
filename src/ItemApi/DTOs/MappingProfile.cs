using AutoMapper;
using ItemApi.Models;

namespace ItemApi.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ItemDTO, Item>();
            CreateMap<Item, ItemDTO>();
            CreateMap<CategoryDTO, Category>();
            CreateMap<Category, CategoryDTO>();
        }

    }
}
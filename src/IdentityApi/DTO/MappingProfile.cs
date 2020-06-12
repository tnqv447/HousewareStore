using AutoMapper;
using IdentityApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityApi.DTO
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserDTO>()
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(src => src.Id)
                );


            CreateMap<ApplicationUserDTO, ApplicationUser>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.UserId)
                );
        }

    }
}
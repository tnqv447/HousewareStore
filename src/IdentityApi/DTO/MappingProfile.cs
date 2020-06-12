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
                )
                .ForMember(
                    dest => dest.PhoneNumber,
                    opt => opt.MapFrom(src => src.PhoneNumberStr)
                )
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.EmailStr)
                );

            CreateMap<ApplicationUserDTO, ApplicationUser>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.UserId)
                )
                .ForMember(
                    dest => dest.PhoneNumberStr,
                    opt => opt.MapFrom(src => src.PhoneNumber)
                )
                .ForMember(
                    dest => dest.EmailStr,
                    opt => opt.MapFrom(src => src.Email)
                );
        }

    }
}
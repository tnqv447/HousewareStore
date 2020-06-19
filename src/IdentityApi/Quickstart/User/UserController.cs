using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using IdentityApi.Data.Repos;
using IdentityApi.DTO;
using IdentityApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IdentityApi.Quickstart.User
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    //[Authorize(Roles = "Administrators")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //private readonly AppSettings _settings;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepo,
            //IOptions<AppSettings> settings, 
            IMapper mapper)
        {
            //_settings = settings.Value;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUserDTO>> GetUser(string id)
        {
            if (!this.ValidateRole(new string[] { "Administrators" }))
                return Forbid();
            var user = await _userRepo.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }
            var dto = toDto(user);
            dto.Role = await _userRepo.GetRoleByUser(user.Id);
            return dto;
        }

        [HttpGet("manage")]
        public async Task<ActionResult<List<ApplicationUserDTO>>> ManagerUser(string role = null, string name = null, string username = null, SortType sortType = SortType.Role, SortOrder sortOrder = SortOrder.Ascending)
        {
            if (!this.ValidateRole(new string[] { "Administrators", "Managers" }))
                return Forbid();
            IList<ApplicationUser> users = null;
            if (String.IsNullOrEmpty(role))
            {
                users = await _userRepo.GetAllUser();
            }
            else
            {
                users = await _userRepo.GetUsersByRole(role);
            }

            if (!String.IsNullOrEmpty(name))
            {
                users = users.Where(m => m.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!String.IsNullOrEmpty(username))
            {
                users = users.Where(m => m.UserName.Contains(username, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var dtos = new List<ApplicationUserDTO>(toDtoRange(users));
            if (!String.IsNullOrEmpty(role))
            {
                foreach (var dto in dtos)
                    dto.Role = role;
            }
            else
            {
                foreach (var dto in dtos)
                {
                    dto.Role = await _userRepo.GetRoleByUser(dto.UserId);
                }
            }
            dtos = this.SortUser(dtos, sortType, sortOrder);

            return dtos;
        }
        [HttpGet("sales")]
        public async Task<ActionResult<List<ApplicationUserDTO>>> GetSales()
        {
            IList<ApplicationUser> users = null;
            users = await _userRepo.GetUsersByRole("Sales");
            var dtos = new List<ApplicationUserDTO>(toDtoRange(users));
            foreach (var dto in dtos)
                dto.Role = "Sales";
            return dtos;
        }
        [HttpPost]
        public async Task<ActionResult<ApplicationUser>> Create(ApplicationUserDTO dto)
        {
            //var user = this.toEntity(dto);
            if (!this.ValidateRole(new string[] { "Administrators" }))
                return Forbid();
            if (!String.IsNullOrEmpty(dto.UserName) && !String.IsNullOrEmpty(dto.Password))
            {
                await _userRepo.CreateUser(dto);
                return CreatedAtAction(nameof(GetUser), new { id = dto.UserId }, dto);
            }
            return NotFound();
        }

        [HttpGet("changepassword")]
        public async Task<ActionResult<UserChangePassword>> ChangePassword(string userId, UserChangePassword dto)
        {
            if (userId != dto.UserId)
            {
                return BadRequest();
            }
            var user = await _userRepo.GetUser(userId);
            if (user == null)
            {
                return NotFound();
            }
            var res = false;
            try
            {
                res = await _userRepo.ChangePassword(user, dto.Password, dto.NewPassword);
            }
            catch
            {
                return NotFound();
            }
            dto.Succeed = res;
            return dto;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, ApplicationUserDTO dto)
        {
            // var Item = await _itemRepos.GetBy(id);
            var isExisted = _userRepo.UserExists(id);
            if (!isExisted)
            {
                return NotFound();
            }

            // _mapper.Map<Item>(ItemDTO);

            try
            {
                await _userRepo.UpdateUser(dto);
            }
            catch (DbUpdateConcurrencyException) when (!_userRepo.UserExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!this.ValidateRole(new string[] { "Administrators" }))
                return Forbid();
            var user = await _userRepo.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            await _userRepo.DeleteUser(user);

            return NoContent();
        }

        //
        //support functions
        //
        private List<ApplicationUserDTO> SortUser(List<ApplicationUserDTO> dtos, SortType sortType, SortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case (SortOrder.Descending):
                    {
                        switch (sortType)
                        {
                            case (SortType.UserId):
                                dtos = dtos.OrderByDescending(m => m.UserId).ToList();
                                break;
                            case (SortType.FullName):
                                dtos = dtos.OrderByDescending(m => m.Name.ToLower()).ToList();
                                break;
                            case (SortType.GivenName):
                                dtos = dtos.OrderByDescending(m => m.GivenName.ToLower()).ToList();
                                break;
                            case (SortType.FamilyName):
                                dtos = dtos.OrderByDescending(m => m.FamilyName.ToLower()).ToList();
                                break;
                            case (SortType.Username):
                                dtos = dtos.OrderByDescending(m => m.UserName).ToList();
                                break;
                            default:
                                dtos = dtos.OrderByDescending(m => m.Role).ToList();
                                break;
                        }
                        break;
                    }
                default:
                    {
                        switch (sortType)
                        {
                            case (SortType.UserId):
                                dtos = dtos.OrderBy(m => m.UserId).ToList();
                                break;
                            case (SortType.FullName):
                                dtos = dtos.OrderBy(m => m.Name.ToLower()).ToList();
                                break;
                            case (SortType.GivenName):
                                dtos = dtos.OrderBy(m => m.GivenName.ToLower()).ToList();
                                break;
                            case (SortType.FamilyName):
                                dtos = dtos.OrderBy(m => m.FamilyName.ToLower()).ToList();
                                break;
                            case (SortType.Username):
                                dtos = dtos.OrderBy(m => m.UserName).ToList();
                                break;
                            default:
                                dtos = dtos.OrderBy(m => m.Role).ToList();
                                break;
                        }
                        break;
                    }

            }
            return dtos;
        }

        //
        //mapping support functions
        //
        private bool ValidateRole(string[] roles)
        {
            var roleClaim = User.Claims.Where(m => m.Type.Equals(ClaimTypes.Role)).FirstOrDefault().Value;
            for (int i = 0; i < roles.Count(); i++)
            {
                if (roleClaim.Equals(roles[i])) return true;
            }
            return false;
        }

        private ApplicationUserDTO toDto(ApplicationUser user)
        {
            return _mapper.Map<ApplicationUser, ApplicationUserDTO>(user);
        }
        private ApplicationUser toEntity(ApplicationUserDTO dto)
        {
            return _mapper.Map<ApplicationUserDTO, ApplicationUser>(dto);
        }
        private IList<ApplicationUserDTO> toDtoRange(IList<ApplicationUser> users)
        {
            return _mapper.Map<IList<ApplicationUser>, IList<ApplicationUserDTO>>(users);
        }
        private IList<ApplicationUser> toEntityRange(IList<ApplicationUserDTO> dtos)
        {
            return _mapper.Map<IList<ApplicationUserDTO>, IList<ApplicationUser>>(dtos);
        }

    }
}
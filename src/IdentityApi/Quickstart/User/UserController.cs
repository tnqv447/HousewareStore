using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


using IdentityApi.Models;
using IdentityApi.DTO;
using IdentityApi.Data.Repos;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.EntityFrameworkCore;

namespace IdentityApi.Quickstart.User
{
    //[Authorize(Roles = "Managers, Administrators")]
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
            var user = await _userRepo.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }
            return toDto(user);
        }

        [HttpGet("manage")]
        public async Task<ActionResult<List<ApplicationUserDTO>>> ManagerUser(string role = null)
        {
            IList<ApplicationUser> users = null;
            if (String.IsNullOrEmpty(role))
            {
                users = await _userRepo.GetAllUser();
            }
            else
            {
                users = await _userRepo.GetUsersByRole(role);
            }

            var dtos = new List<ApplicationUserDTO>(toDtoRange(users));
            if (!String.IsNullOrEmpty(role))
            {
                foreach (var dto in dtos)
                    dto.Role = role;
            }
            return dtos;
        }

        [HttpPost]
        public async Task<ActionResult<ApplicationUser>> Create(ApplicationUserDTO dto)
        {
            //var user = this.toEntity(dto);
            if (!String.IsNullOrEmpty(dto.UserName) && !String.IsNullOrEmpty(dto.Password))
            {
                await _userRepo.CreateUser(dto);
                return CreatedAtAction(nameof(GetUser), new { id = dto.UserId }, dto);
            }
            return NotFound();

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, ApplicationUserDTO dto)
        {
            if (id != dto.UserId)
            {
                return BadRequest();
            }

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
            var user = await _userRepo.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            await _userRepo.DeleteUser(user);

            return NoContent();
        }

        //
        //mapping support functions
        //
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
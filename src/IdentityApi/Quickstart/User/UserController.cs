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
    [Authorize(Roles = "Managers, Administrators")]
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
            var user = new List<ApplicationUser>();
            if (String.IsNullOrEmpty(role))
            {
                user = await _userRepo.GetAllUser();
            }
            else
            {
                user = await _userRepo.GetUsersByRole(role);
            }

            user = user.OrderBy(m => m.Role).ThenBy(m => m.UserName).ToList();

            return toDtoRange(user);
        }

        [HttpPost]
        public async Task<ActionResult<ApplicationUser>> Create(ApplicationUserDTO dto)
        {
            var user = this.toEntity(dto);
            if (!String.IsNullOrEmpty(user.UserName) && !String.IsNullOrEmpty(user.Password))
            {
                await _userRepo.CreateUser(user);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
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
            var user = toEntity(dto);
            if (user == null)
            {
                return NotFound();
            }

            // _mapper.Map<Item>(ItemDTO);

            try
            {
                await _userRepo.Update(user);
            }
            catch (DbUpdateConcurrencyException) when (!_userRepo.UserExists(id).Result)
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

            await _userRepo.Delete(user);

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
        private List<ApplicationUserDTO> toDtoRange(List<ApplicationUser> users)
        {
            return _mapper.Map<List<ApplicationUser>, List<ApplicationUserDTO>>(users);
        }
        private List<ApplicationUser> toEntityRange(List<ApplicationUserDTO> dtos)
        {
            return _mapper.Map<List<ApplicationUserDTO>, List<ApplicationUser>>(dtos);
        }

    }
}
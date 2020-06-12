using IdentityApi.Models;
using IdentityApi.DTO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IdentityApi.Data.Repos
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser> GetUser(string id);
        Task<IList<ApplicationUser>> GetAllUser();
        Task<IList<ApplicationUser>> GetUsersByRole(string role);
        Task<bool> UpdateUser(ApplicationUserDTO dto);
        Task<bool> CreateUser(ApplicationUserDTO dto);
        Task<bool> DeleteUser(ApplicationUser user);
        Task<bool> UserExists(string id);

    }
}
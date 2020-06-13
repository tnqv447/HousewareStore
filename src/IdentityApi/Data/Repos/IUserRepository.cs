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

        Task<ApplicationUser> CreateUser(ApplicationUserDTO dto);
        Task DeleteUser(ApplicationUser user);
        Task UpdateUser(ApplicationUserDTO dto);
        bool UserExists(string id);

    }
}
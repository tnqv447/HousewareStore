using IdentityApi.Models;
using IdentityApi.DTO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IdentityApi.Data.Repos
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser> GetUser(string id);
        Task<List<ApplicationUser>> GetAllUser();
        Task<List<ApplicationUser>> GetUsersByRole(string role);
        Task UpdateUser(ApplicationUser user);
        Task<bool> CreateUser(ApplicationUser user);
        Task<bool> DeleteUser(ApplicationUser user);
        Task<bool> UserExists(string id);

    }
}
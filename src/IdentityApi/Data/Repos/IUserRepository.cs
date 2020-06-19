using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityApi.DTO;
using IdentityApi.Models;

namespace IdentityApi.Data.Repos {
    public interface IUserRepository : IRepository<ApplicationUser> {
        Task<ApplicationUser> GetUser (string id);
        Task<IList<ApplicationUser>> GetAllUser ();
        Task<IList<ApplicationUser>> GetUsersByRole (string role);

        Task<ApplicationUser> CreateUser (ApplicationUserDTO dto);
        Task DeleteUser (ApplicationUser user);
        Task UpdateUser (ApplicationUserDTO dto);

        Task<string> GetRoleByUser (string id);
        Task<bool> ChangePassword (ApplicationUser user, string password, string newPassword);
        bool UserExists (string id);

    }
}
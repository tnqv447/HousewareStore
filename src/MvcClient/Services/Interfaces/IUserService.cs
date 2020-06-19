using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvcClient.Models;
using MvcClient.ViewModels;

namespace MvcClient.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> ManageUsers(string role = null, string name = null, string username = null, SortType sortType = SortType.Role, SortOrder sortOrder = SortOrder.Ascending);
        Task<User> GetUser(string id);
        Task CreateUser(User item);
        Task UpdateUser(string id, User item);
        Task DeleteUser(string id);
        Task<IEnumerable<User>> GetSales();
        Task<UserChangePassword> ChangePassword(string userId, UserChangePassword user);
    }
}
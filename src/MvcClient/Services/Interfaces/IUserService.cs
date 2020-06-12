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
        Task<IEnumerable<User>> ManageUsers(string role = null);
        Task<User> GetUser(string id);
        Task CreateUser(User item);
        Task UpdateUser(string id, User item);
        Task DeleteUser(string id);
    }
}
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MvcClient.Infrastructure;
using MvcClient.Models;
using System;
namespace MvcClient.Services
{
    public class UserService : IUserService
    {

        private readonly string _baseUrl;
        private readonly IHttpClient _httpClient;

        public UserService(IHttpClient httpClient, IOptions<AppSettings> appSettings)
        {
            _httpClient = httpClient;
            _baseUrl = $"{appSettings.Value.IdentityUrl}/api/user";
        }

        public async Task<IEnumerable<User>> ManageUsers(string role = null, string name = null, string username = null, SortType sortType = SortType.Role, SortOrder sortOrder = SortOrder.Ascending)
        {
            var url = _baseUrl + $"/manage?role={role}&name={name}&username={username}&sortType={sortType}&sortOrder={sortOrder}";

            return await _httpClient.GetListAsync<User>(url);
        }

        public async Task<User> GetUser(string id)
        {
            var url = _baseUrl + $"/{id}";

            return await _httpClient.GetAsync<User>(url);
        }

        public async Task CreateUser(User user)
        {
            var url = _baseUrl;

            await _httpClient.PostAsync<User>(url, user);
        }
        public async Task<UserChangePassword> ChangePassword(string userId, UserChangePassword user)
        {
            var url = _baseUrl + $"/changepassword?userId={userId}";

            return await _httpClient.GetAsync<UserChangePassword>(url);
        }
        public async Task UpdateUser(string id, User user)
        {
            var url = _baseUrl + $"/{id}";

            await _httpClient.PutAsync<User>(url, user);
        }
        public async Task DeleteUser(string id)
        {
            var url = _baseUrl + $"/{id}";

            await _httpClient.DeleteAsync(url);
        }

    }
}
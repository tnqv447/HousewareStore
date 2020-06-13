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

        public async Task<IEnumerable<User>> ManageUsers(string role = null)
        {
            var url = _baseUrl + $"/manage?role={role}";

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
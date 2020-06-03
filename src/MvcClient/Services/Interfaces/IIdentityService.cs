using System.Security.Claims;

namespace MvcClient.Services
{
    public interface IIdentityService<T> where T : class
    {
        T Get(ClaimsPrincipal user);
    }
}
using System.Security.Claims;
using MvcClient.Models;
using Newtonsoft.Json;

namespace MvcClient.Services
{
    public class IdentityService : IIdentityService<Buyer>
    {
        public Buyer Get(ClaimsPrincipal user)
        {
            return new Buyer
            {
                Id = user.FindFirstValue("sub"),
                FirstName = user.FindFirstValue("firstname"),
                LastName = user.FindFirstValue("lastname"),
                PhoneNumber = user.FindFirstValue("phonenumber"),
                PictureUrl = user.FindFirstValue("pictureurl"),
                Email = user.FindFirstValue("email"),
                UserName = user.FindFirstValue("name"),
                Address = JsonConvert.DeserializeObject<Address>(user.FindFirstValue("address"))
            };
        }
    }
}
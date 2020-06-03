using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityApi.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace IdentityApi.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim("name", user.UserName),
                new Claim("firstname", GetClaimValue(userClaims, "given_name")),
                new Claim("lastname", GetClaimValue(userClaims, "family_name")),
                new Claim("address", GetClaimValue(userClaims, "address"))
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            context.IssuedClaims.AddRange(claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);

            context.IsActive = (user != null) &&
                (!user.LockoutEnd.HasValue || user.LockoutEnd.Value <= DateTime.Now);
        }

        private static string GetClaimValue(IList<Claim> userClaims, string claimType)
        {
            return userClaims.FirstOrDefault(c => c.Type == claimType).Value;
        }
    }
}
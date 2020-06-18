using System.Xml;
using System;
using System.Net.Http;
using System.Text;
using System.Runtime.Serialization;
using IdentityApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;


using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Newtonsoft.Json;
using Serilog;
using IdentityApi.DTO;

namespace IdentityApi.Data.Repos
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(context)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            //await this.TransferClaimsToUser(user);
            return user;
        }

        public async Task<IList<ApplicationUser>> GetAllUser()
        {
            var users = await this.GetAll();
            // foreach (var user in users)
            // {
            //     await this.TransferClaimsToUser(user);
            // }
            return users;
        }

        public async Task<IList<ApplicationUser>> GetUsersByRole(string role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            // foreach (var user in users)
            // {
            //     await this.TransferClaimsToUser(user);
            // }
            return users;
        }

        public async Task UpdateUser(ApplicationUserDTO dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            this.TransferDataToUser(dto, user);
            var res = await _userManager.UpdateAsync(user);
            if (res.Succeeded)
            {
                Log.Debug($"{dto.UserName} updated");
            }
            else
            {
                Log.Debug($"Update {dto.UserName} failed. Some errors happened");
            }

            // var claims = await .up.GetClaimsAsync(await _userManager.FindByIdAsync(user.Id));
            // var replace = this.TransferUserToClaims(user);

            // //update claims
            // foreach (var temp in replace)
            // {
            //     var toBeReplaced = GetClaim(claims, temp.Type);
            //     if (!toBeReplaced.Value.Equals(temp.Value))
            //         await _userManager.ReplaceClaimAsync(user, toBeReplaced, temp);
            // }

        }
        public async Task<ApplicationUser> CreateUser(ApplicationUserDTO dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            var success = true;
            if (user == null)
            {
                user = new ApplicationUser();
                this.TransferDataToUser(dto, user);
                var result = _userManager.CreateAsync(user, dto.Password).Result;
                if (!result.Succeeded)
                {
                    success = false;
                    throw new Exception(result.Errors.First().Description);
                }

                result = _userManager.AddToRoleAsync(user, dto.Role).Result;
                if (!result.Succeeded)
                {
                    success = false;
                    throw new Exception(result.Errors.First().Description);
                }
                if (success)
                {
                    Log.Debug($"{dto.UserName} created");
                    return user;
                }
                else Log.Debug($"Update {dto.UserName} failed. Some errors happened");

                return null;
            }
            else
            {
                Log.Debug($"{dto.UserName} already exists");
                return null;
            }
        }
        public async Task DeleteUser(ApplicationUser user)
        {
            if (user != null)
            {
                // var claims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);
                var username = user.UserName;
                var success = true;

                var res = await _userManager.RemoveFromRolesAsync(user, roles);
                if (!res.Succeeded)
                {
                    success = false;
                    throw new Exception(res.Errors.First().Description);
                }

                // res = await _userManager.RemoveClaimsAsync(user, claims);
                // if (!res.Succeeded)
                // {
                //     success = false;
                //     throw new Exception(res.Errors.First().Description);
                // }

                res = await _userManager.DeleteAsync(user);
                if (!res.Succeeded)
                {
                    success = false;
                    throw new Exception(res.Errors.First().Description);
                }
                if (success)
                {
                    Log.Debug($"{username} deleted successfully");
                }
                else Log.Debug($"Delete {username} failed. Some errors happened");

            }

            Log.Debug($"Application user doesn't exist. Delete failed");

        }
        public async Task<string> GetRoleByUser(string id)
        {
            return (await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(id))).FirstOrDefault();
        }

        public bool UserExists(string id)
        {
            return (_userManager.FindByIdAsync(id).Result) == null ? false : true;
        }

        //mapping support 

        // private async Task TransferClaimsToUser(ApplicationUser user)
        // {
        //     var claims = await _userManager.GetClaimsAsync(user);
        //     user.Name = GetClaimValue(claims, "name");
        //     user.GivenName = GetClaimValue(claims, "given_name");
        //     user.FamilyName = GetClaimValue(claims, "family_name");
        //     user.PhoneNumber = GetClaimValue(claims, "phone_number");
        //     user.Email = GetClaimValue(claims, "email");
        //     user.Website = GetClaimValue(claims, "website");
        //     user.PictureUrl = GetClaimValue(claims, "picture");
        //     user.Address = GetClaimValue(claims, "address");
        //     user.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
        // }

        // private List<Claim> TransferUserToClaims(ApplicationUser user)
        // {
        //     return new List<Claim>{
        //         new Claim("name", user.Name),
        //         new Claim("given_name", user.GivenName),
        //         new Claim("family_name", user.FamilyName),
        //         new Claim("phone_number", user.PhoneNumberStr),
        //         new Claim("email", user.EmailStr),
        //         new Claim("website", user.Website),
        //         new Claim("picture", user.PictureUrl),
        //         new Claim("address", user.Address)
        //     };
        // }
        private void TransferDataToUser(ApplicationUserDTO dto, ApplicationUser user)
        {
            if (String.IsNullOrEmpty(user.Id) && !String.IsNullOrEmpty(dto.UserId))
                user.Id = dto.UserId;
            user.UserName = dto.UserName;
            user.Name = dto.Name;
            user.GivenName = dto.GivenName;
            user.FamilyName = dto.FamilyName;
            user.Email = dto.Email;
            user.EmailConfirmed = true;
            user.PhoneNumber = dto.PhoneNumber;
            user.PhoneNumberConfirmed = true;
            user.PictureUrl = dto.PictureUrl;
            user.Website = dto.Website;
            user.Address = dto.Address;
        }

        private static string GetClaimValue(IList<Claim> userClaims, string claimType)
        {
            return userClaims.FirstOrDefault(c => c.Type == claimType).Value;
        }

        private static Claim GetClaim(IList<Claim> userClaims, string claimType)
        {
            return userClaims.FirstOrDefault(c => c.Type == claimType);
        }





    }
}
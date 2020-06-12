using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace IdentityApi.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        //for claims
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }

        public string EmailStr { get; set; }
        public string PhoneNumberStr { get; set; }

        public string PictureUrl { get; set; }
        public string Website { get; set; }

        public string Address { get; set; }

        public string Role { get; set; }

        //for add, update user
        //public string UserNameStr { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }


    }

}

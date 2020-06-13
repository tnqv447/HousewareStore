using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using IdentityApi.Models;
namespace IdentityApi.DTO
{
    public class ApplicationUserDTO
    {
        //[JsonProperty("user_id")]
        public string UserId { get; set; }

        //for claims
        public string Name { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Family name")]
        //[JsonProperty("family_name")]
        public string FamilyName { get; set; }

        [Display(Name = "Given name")]
        //[JsonProperty("give_name")]
        public string GivenName { get; set; }

        public string Email { get; set; }

        [Display(Name = "Phone number")]
        //[JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        //[JsonProperty("picture_url")]
        public string PictureUrl { get; set; }

        public string Website { get; set; }

        public Address Address { get; set; }


        //for add,update user
        //public string UserNameStr { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
    }


}
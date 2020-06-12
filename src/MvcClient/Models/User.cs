using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace MvcClient.Models
{
    public class User
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        //for claims
        public string Name { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Family name")]
        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        [Display(Name = "Given name")]
        [JsonProperty("give_name")]
        public string GivenName { get; set; }

        public string Email { get; set; }

        [Display(Name = "Phone number")]
        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("picture_url")]
        public string PictureUrl { get; set; }

        public string Website { get; set; }

        public Address Address { get; set; }

        public string Role { get; set; }


        //for add,update user
        //public string UserNameStr { get; set; }
        public string Password { get; set; }
        public string getFullName()
        {
            return Name + " " + FamilyName + " " + GivenName;
        }
    }

    public class Address
    {
        [JsonProperty("street_address")]
        [Display(Name = "Street address")]
        public string StreetAddress { get; set; }

        [Display(Name = "Locality/District")]
        public string Locality { get; set; }

        public string City { get; set; }
        public string Country { get; set; }

        [JsonProperty("postal_code")]
        [Display(Name = "Postal code")]
        public string PostalCode { get; set; }

        public override string ToString()
        {
            return $"{StreetAddress}, {Locality}, {City}, {Country}, {PostalCode}";
        }
    }
}
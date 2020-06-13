using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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

        public string PictureUrl { get; set; }
        public string Website { get; set; }

        public Address Address { get; set; }

        //public string Role { get; set; }

        //for add, update user
        //public string UserNameStr { get; set; }
        // public string Password { get; set; }
        // public string NewPassword { get; set; }

    }

    [Owned]
    public class Address
    {
        //[JsonProperty("street_address")]
        [Display(Name = "Street address")]
        public string StreetAddress { get; set; }

        [Display(Name = "Locality/District")]
        public string Locality { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        //[JsonProperty("postal_code")]
        [Display(Name = "Postal code")]
        public string PostalCode { get; set; }

        public Address()
        {

        }
        public Address(string StreetAddress, string Locality, string City, string Country, string PostalCode)
        {
            this.StreetAddress = StreetAddress;
            this.Locality = Locality;
            this.City = City;
            this.Country = Country;
            this.PostalCode = PostalCode;
        }

        public override string ToString()
        {
            return $"{StreetAddress}, {Locality}, {City}, {Country}, {PostalCode}";
        }
    }

}

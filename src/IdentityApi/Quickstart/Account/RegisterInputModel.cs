using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace IdentityServer4.Quickstart.UI
{
    public class RegisterInputModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
        public bool RememberMe { get; set; }
        public string Role { get; set; }
        public string ReturnUrl { get; set; }
        public UserProfileInputModel UserProfile { get; set; }

        // public RegisterInputModel()
        // {
        //     UserProfile = new UserProfileInputModel();
        // }

    }

    public class UserProfileInputModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Given name")]
        public string GivenName { get; set; }

        [Required]
        [Display(Name = "Family name")]
        public string FamilyName { get; set; }

        [Required]
        [Display(Name = "Phone number")]
        [RegularExpression(@"^[0-9]{10}$")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }

        [Display(Name = "Picture")]
        public string PictureUrl { get; set; }

        public string Website { get; set; }
        public Address Address { get; set; }

        // public UserProfileInputModel()
        // {
        //     Address = new Address();
        // }

    }

    public class Address
    {
        [JsonProperty("street_address")]
        [Display(Name = "Street address")]
        public string StreetAddress { get; set; }

        //phuong va quan,huyen
        [Display(Name = "Locality/District")]
        public string Locality { get; set; }

        public string City { get; set; }
        public string Country { get; set; }

        [JsonProperty("postal_code")]
        [Display(Name = "Postal code")]
        public string PostalCode { get; set; }
    }
}
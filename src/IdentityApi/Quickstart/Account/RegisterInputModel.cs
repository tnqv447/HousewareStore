using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using IdentityApi.Models;

namespace IdentityServer4.Quickstart.UI
{
    public class RegisterInputModel
    {
        [Required]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "The Password cannot be empty.")]
        [DisplayName("Password")]
        [RegularExpression(@"^(?:(?:(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]))|(?:(?=.*[a-z])(?=.*[A-Z])(?=.*[*.!@$%^&(){}[]:;<>,.?/~_+-=|\]))|(?:(?=.*[0-9])(?=.*[A-Z])(?=.*[*.!@$%^&(){}[]:;<>,.?/~_+-=|\]))|(?:(?=.*[0-9])(?=.*[a-z])(?=.*[*.!@$%^&(){}[]:;<>,.?/~_+-=|\]))).{8,32}$", ErrorMessage = "At least one uppercase character,one lowercase character, one number, one special character and between 8 to 32 characters in length")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [MaxLength(100, ErrorMessage = "The Password cannot be longer than 100 characters.")]
        [Compare("Password", ErrorMessage = "The entered passwords do not match.")]
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
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Your email is invalid")]
        public string Email { get; set; }

        [Display(Name = "Picture")]
        public string PictureUrl { get; set; }

        public string Website { get; set; }
        public Address Address { get; set; }

        public UserProfileInputModel() { }
        public UserProfileInputModel(string Name, string GivenName, string FamilyName, string PhoneNumber, string Email, string PictureUrl, Address address)
        {
            this.Name = Name;
            this.GivenName = GivenName;
            this.FamilyName = FamilyName;
            this.PhoneNumber = PhoneNumber;
            this.Email = Email;
            this.PictureUrl = PictureUrl;
            this.Address = address;
        }
    }

    // public class Address
    // {
    //     [JsonProperty("street_address")]
    //     [Display(Name = "Street address")]
    //     public string StreetAddress { get; set; }

    //     //phuong va quan,huyen
    //     [Display(Name = "Locality/District")]
    //     public string Locality { get; set; }

    //     public string City { get; set; }
    //     public string Country { get; set; }

    //     [JsonProperty("postal_code")]
    //     [Display(Name = "Postal code")]
    //     public string PostalCode { get; set; }
    //     public Address()
    //     {

    //     }
    //     public Address(string StreetAddress, string Locality, string City, string Country, string PostalCode)
    //     {
    //         this.StreetAddress = StreetAddress;
    //         this.Locality = Locality;
    //         this.City = City;
    //         this.Country = Country;
    //         this.PostalCode = PostalCode;
    //     }
    // }
}
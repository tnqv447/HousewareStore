using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace MvcClient.Models
{
    public class User
    {
        //[JsonProperty("user_id")]
        public string UserId { get; set; }

        //for claims
        public string Name { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Display(Name = "Family name")]
        //[JsonProperty("family_name")]
        public string FamilyName { get; set; }

        [Required]
        [Display(Name = "Given name")]
        // [JsonProperty("give_name")]
        public string GivenName { get; set; }
        [Required]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Your email is invalid")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Phone number")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Your phone is invalid")]
        public string PhoneNumber { get; set; }

        //[JsonProperty("picture_url")]
        public string PictureUrl { get; set; }

        public string Website { get; set; }

        public Address Address { get; set; }

        public string Role { get; set; }


        //for add,update user
        //public string UserNameStr { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression(@"^(?:(?:(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]))|(?:(?=.*[a-z])(?=.*[A-Z])(?=.*[*.!@$%^&(){}[]:;<>,.?/~_+-=|\]))|(?:(?=.*[0-9])(?=.*[A-Z])(?=.*[*.!@$%^&(){}[]:;<>,.?/~_+-=|\]))|(?:(?=.*[0-9])(?=.*[a-z])(?=.*[*.!@$%^&(){}[]:;<>,.?/~_+-=|\]))).{8,32}$", ErrorMessage = "At least one uppercase character,one lowercase character, one number, one special character and between 8 to 32 characters in length")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [MaxLength(100, ErrorMessage = "The Password cannot be longer than 100 characters.")]
        [Compare("Password", ErrorMessage = "The entered passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public override string ToString()
        {
            return $"{UserId}, {UserName}, {this.Password}, {this.Name}, {this.Email}, {this.PhoneNumber}, {this.PictureUrl}, {this.Address.ToString()}, {this.Website}, {this.Role}";
        }
    }

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

        public Address() { }
        public Address(Address a)
        {
            this.StreetAddress = a.StreetAddress;
            this.Locality = a.Locality;
            this.City = a.City;
            this.Country = a.Country;
            this.PostalCode = a.PostalCode;
        }

        public override string ToString()
        {
            return $"{StreetAddress}, {Locality}, {City}, {Country}, {PostalCode}";
        }
    }

    public enum SortOrder
    {
        Descending = 0,
        Ascending = 1,
    }
    public enum SortType
    {
        UserId = 0,
        FullName = 1,
        GivenName = 2,
        FamilyName = 3,
        Role = 4,
        Username = 5
    }
}
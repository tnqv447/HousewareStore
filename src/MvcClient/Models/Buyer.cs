using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MvcClient.Models
{
    public class Buyer
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public Address Address { get; set; }
        [Required]
        [Display(Name = "Phone number")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Your phone is invalid")]
        public string PhoneNumber { get; set; }
        public string PictureUrl { get; set; }
        public string Website { get; set; }
        public string FullName { get; set; }
        public IFormFile ImageURL { get; set; }
        [Required]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Your email is invalid")]
        public string Email { get; set; }
    }

    // look into class Address in class User
    //
    // public class Address
    // {
    //     public string Street_address { get; set; }
    //     public string Locality { get; set; }
    //     public string City { get; set; }
    //     public string Postal_code { get; set; }
    //     public string Country { get; set; }
    // }
}
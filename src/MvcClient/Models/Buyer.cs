namespace MvcClient.Models
{
    public class Buyer
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public string Street_address { get; set; }
        public string Locality { get; set; }
        public string City { get; set; }
        public string Postal_code { get; set; }
        public string Country { get; set; }

        public override string ToString()
        {
            return $"{Street_address}, {Locality}, {City}, {Postal_code}, {Country}";
        }
    }
}
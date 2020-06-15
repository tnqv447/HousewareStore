namespace MvcClient
{
    public class AppSettings
    {
        //public string MovieUrl { get; set; }
        public string IdentityUrl { get; set; }
        public string ItemUrl { get; set; }
        public string CartUrl { get; set; }
        public string OrderUrl { get; set; }
        public string ExternalCatalogBaseUrl { get; set; }
        //public string ExternalCatalogBaseUrl { get; set; }
        public ClientCredentials ClientCredentials { get; set; }
    }

    public class ClientCredentials
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
    }
    public class StripeSettings
    {
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
    }
}
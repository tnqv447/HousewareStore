namespace IdentityServer4.Quickstart.UI
{
    public class RegisterViewModel : RegisterInputModel
    {
        public bool AllowRememberLogin { get; set; } = true;
        public bool EnableLocalRegister { get; set; } = true;

    }
}
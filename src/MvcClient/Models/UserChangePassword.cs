namespace MvcClient.Models
{
    public class UserChangePassword
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public bool Succeed { get; set; } = false;
    }
}
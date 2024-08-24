namespace ReviewVerse.Shared.Constants;

public readonly struct Constants
{
    public const string SystemName = "System";
    public static readonly string[] ImageTypeAllowed = new string[] { ".png", ".jpg", ".jpeg" };
    public const string DocNameTemp = "temp";
    public static readonly string[] CVExtensionsAllowed = new[] { ".doc", ".docx", ".pdf" };

    public readonly struct CustomClaimTypes
    {
        public const string UserClientId = "user_client_id";
        public const string UserId = "user_id";
        public const string Email = "email";
        public const string Role = "role";
        public const string Phone = "phone";
        public const string Provider = "provider";
        public const string SecurityStamp = "st";
    }

    public readonly struct EmailCode
    {
        public const string ForgotPassword = "ForgotPassword";
        public const string ChangePassword = "ChangePassword";
    }
}
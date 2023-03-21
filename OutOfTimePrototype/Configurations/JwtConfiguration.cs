using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace OutOfTimePrototype.Configurations
{
    public class JwtConfiguration
    {
        public const string Issuer = "ApiJwtIssuer";
        public const string Audience = "ApiJwtAudience";
        public const string SecretKey = "Idet medved' po lesu, vidit, mashina gorit, sel v neye i sgorel";
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        }
    }
}

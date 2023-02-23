using Microsoft.IdentityModel.Tokens;
using OutOfTimePrototype.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace OutOfTimePrototype.Services.Authentication
{
    public class TokenService : ITokenService
    {
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var signingCredentials = new SigningCredentials(JwtConfiguration.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: JwtConfiguration.Issuer,
                audience: JwtConfiguration.Audience,
                signingCredentials: signingCredentials,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(3));

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = JwtConfiguration.Issuer,
                ValidateAudience = true,
                ValidAudience = JwtConfiguration.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = JwtConfiguration.GetSymmetricSecurityKey(),
                ValidateLifetime = false
            };

            SecurityToken secutityToken;
            var principal = (new JwtSecurityTokenHandler()).ValidateToken(token, tokenValidationParameters, out secutityToken);
            var jwtSecurityToken = secutityToken as JwtSecurityToken;
            if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenInvalidTypeException();
            }
            return principal;
        }
    }
}

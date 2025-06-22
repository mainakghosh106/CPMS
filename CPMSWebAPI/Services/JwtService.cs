using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CPMSWebAPI.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration= configuration;
        }

        public string GenerateAccessToken(string username, int userId)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim("UserId", userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            return GenerateToken(claims, true);
        }

        public string GenerateRefreshToken(string username, int userId)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim("UserId", userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            return GenerateToken(claims, false);
        }

        private string GenerateToken(Claim[] claims, bool isAccessToken)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                isAccessToken ? _configuration["JwtSettings:AccessSecret"] : _configuration["JwtSettings:RefreshSecret"]
            ));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = isAccessToken
                ? DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpiryMinutes"]))
                : DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? GetPrincipalFromToken(string token, bool isRefreshToken = false)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                isRefreshToken ? _configuration["Jwt:SecretKey_Refresh"] : _configuration["Jwt:SecretKey"]
            ));

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = key,
                    ValidateLifetime = !isRefreshToken,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}

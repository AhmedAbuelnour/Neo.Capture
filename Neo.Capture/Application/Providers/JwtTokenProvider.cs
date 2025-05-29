using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Neo.Capture.Domain.Entities;
using System.Security.Claims;
using System.Text;

namespace Neo.Capture.Application.Providers
{
    public class JwtResponse
    {
        public required string AccessToken { get; set; }
    }

    public sealed class JwtTokenProvider
    {
        public async Task<JwtResponse> GetJwtAsync(Profile profile, CancellationToken cancellationToken = default)
        {
            return new JwtResponse
            {
                AccessToken = GetAccessToken(profile)
            };
        }

        private static string GetAccessToken(Profile profile)
        {
            List<Claim> claims =
            [
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, profile.Id.ToString()),
            ];


            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddYears(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F")), SecurityAlgorithms.HmacSha256),
                Issuer = "Neo.Capture",
                IssuedAt = DateTime.UtcNow,
            };

            JsonWebTokenHandler jsonWebTokenHandler = new();

            return jsonWebTokenHandler.CreateToken(tokenDescriptor);
        }
    }

}

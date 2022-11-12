using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoanComparer.Application.Services.JwtFeatures
{
    public class JwtHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _jwtSettings;

        public JwtHandler(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtSettings = configuration.GetSection("JwtSettings");
        }

        public SigningCredentials GetSigningCredentials()
        {
            byte[] securityKey = Encoding.UTF8.GetBytes(_jwtSettings["securityKey"]);
            var symetricSecurityKey = new SymmetricSecurityKey(securityKey);
            return new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256);
        }

        public ICollection<Claim> GetClaims(IdentityUser identityUser)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, identityUser.Email)
            };
            return claims;
        }

        public JwtSecurityToken GenerateJwtSecurityToken(SigningCredentials signingCredentials, ICollection<Claim> claims)
        {
            return new JwtSecurityToken(
                issuer: _jwtSettings["validIssuer"],
                audience: _jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings["expiresInMinutes"])),
                signingCredentials: signingCredentials);
        }
    }
}

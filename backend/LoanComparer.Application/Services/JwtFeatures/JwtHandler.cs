using LoanComparer.Application.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoanComparer.Application.Services.JwtFeatures
{
    public class JwtHandler
    {
        private readonly IConfigurationSection _jwtSettings;
        private readonly UserManager<User> _userManager;

        public JwtHandler(IConfiguration configuration, UserManager<User> userManager)
        {
            _jwtSettings = configuration.GetSection("JwtSettings");
            _userManager = userManager;
        }

        public SigningCredentials GetSigningCredentials()
        {
            byte[] securityKey = Encoding.UTF8.GetBytes(_jwtSettings["securityKey"]);
            var symetricSecurityKey = new SymmetricSecurityKey(securityKey);
            return new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256);
        }

        public async Task<ICollection<Claim>> GetClaimsAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            IList<string> roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }

        public JwtSecurityToken GenerateJwtSecurityToken(SigningCredentials signingCredentials, ICollection<Claim> claims)
        {
            return new JwtSecurityToken(
                issuer: _jwtSettings["validIssuer"],
                audience: _jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddDays(Convert.ToDouble(_jwtSettings["expiresInDays"])),
                signingCredentials: signingCredentials);
        }
    }
}

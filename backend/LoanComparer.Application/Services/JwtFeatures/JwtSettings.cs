using Microsoft.Extensions.Configuration;
using System.Text;

namespace LoanComparer.Application.Services.JwtFeatures
{
    public class JwtSettings
    {
        public byte[] SecurityKey { get; }
        public string ValidIssuer { get; }
        public string ValidAudience { get; }
        internal double ExpiresInDays { get; }

        public JwtSettings(IConfigurationSection configurationSection)
        {
            SecurityKey = Encoding.UTF8.GetBytes(configurationSection["securityKey"]);
            ValidIssuer = configurationSection["validIssuer"];
            ValidAudience = configurationSection["validAudience"];
            ExpiresInDays = Convert.ToDouble(configurationSection["expiresInDays"]);
        }
    }
}

using Inovatec.OfficeManagementTool.Common;
using Inovatec.OfficeManagementTool.Models.Entity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inovatec.OfficeManagementTool.ImplementationsBL.Helpers
{
    public class JwtService
    {
        public static async Task<string> GetToken(User user)
        {
            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, ConfigProvider.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("Id", user.Id.ToString()),
                new Claim("OfficeId", user.OfficeId.ToString() ?? ""),
                new Claim(ClaimTypes.Name, string.Format("{0} {1}", user.FirstName, user.LastName))
            };

            SymmetricSecurityKey key = new (Encoding.UTF8.GetBytes(ConfigProvider.Key));
            SigningCredentials signIn = new (key, SecurityAlgorithms.HmacSha512Signature);

            JwtSecurityToken token = new JwtSecurityToken
            (
                ConfigProvider.Issuer,
                ConfigProvider.Audience,
                claims,
                expires: DateTime.UtcNow.AddHours((double)ConfigProvider.LifetimeHours),
                signingCredentials: signIn
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
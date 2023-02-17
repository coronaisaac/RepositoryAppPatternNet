using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AppDaltonCatalogo.Infrastructure.SQL.Helpers
{
    public class GenerateTokenHelpers
    {
        public static string GenerateToken(string Email, string Name, string LastAcces, string SecretEncodeToken)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("Email", Email),
                new Claim("Name", Name ?? ""),
                new Claim("LastAcces", LastAcces),

            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(SecretEncodeToken));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

    }
}

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace servicer.API.Services
{
    public class TokenService : ITokenService
    {
        private string token;
        private readonly IConfiguration _configuration;

        private JwtSecurityToken _decodedToken;
        private JwtSecurityToken DecodedToken
        {
            get
            {
                if (this._decodedToken == null)
                {
                    _decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                }

                return this._decodedToken;
            }
        }

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(Claim[] claims, DateTime expiryDate)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiryDate,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public bool IsTokenExpired(string token)
        {
            this.token = token;
            return DecodedToken.ValidTo < DateTime.Now.AddMinutes(1);
        }

        public string GetTokenClaim(string token, string claimName)
        {
            this.token = token;
            return DecodedToken.Claims.First(claim => claim.Type == claimName).Value;
        }
    }
}
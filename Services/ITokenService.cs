using System;
using System.Security.Claims;

namespace servicer.API.Services
{
    public interface ITokenService
    {
        string CreateToken(Claim[] claims, DateTime expiryDate);
        bool IsTokenExpired(string token);
        String GetTokenClaim(string token, string claimName);
    }
}
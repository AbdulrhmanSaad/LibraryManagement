using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IRefreshTokenRepository _refreshTokenRepo;

    public TokenService(IConfiguration configuration, IRefreshTokenRepository refreshTokenRepo)
    {
        _configuration = configuration; _refreshTokenRepo = refreshTokenRepo;
    }

    public (string Token, DateTime ExpiresAt) GenerateJwtToken(AppUser user, IList<string> roles, string jwtId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "SuperSecretKeyForDevelopment12345678!"));
        var expiresAt = DateTime.UtcNow.AddMinutes(15);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, jwtId),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? ""),
            new(ClaimTypes.Email, user.Email ?? ""),
            new("fullName", user.FullName)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        return (new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            _configuration["Jwt:Issuer"] ?? "LibraryManagement",
            _configuration["Jwt:Audience"] ?? "LibraryManagementClient",
            claims, expires: expiresAt,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256))), expiresAt);
    }

    public async Task<RefreshToken> GenerateRefreshToken(int userId, string jwtId)
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            AppUserId = userId,
            JwtId = jwtId,
            IsUsed = false,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _refreshTokenRepo.AddAsync(refreshToken);
        return refreshToken;
    }
}
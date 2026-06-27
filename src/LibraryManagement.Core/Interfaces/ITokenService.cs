using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateJwtToken(AppUser user, IList<string> roles, string jwtId);
    Task<RefreshToken> GenerateRefreshToken(int userId, string jwtId);
}
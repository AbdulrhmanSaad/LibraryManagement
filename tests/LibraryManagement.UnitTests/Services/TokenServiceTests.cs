using Microsoft.Extensions.Configuration;
using Moq;
using FluentAssertions;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Services;

namespace LibraryManagement.UnitTests.Services;

public class TokenServiceTests
{
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock;
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        _configMock = new Mock<IConfiguration>();
        _refreshTokenRepoMock = new Mock<IRefreshTokenRepository>();

        _configMock.Setup(x => x["Jwt:Key"]).Returns("SuperSecretKeyForDevelopment12345678!");
        _configMock.Setup(x => x["Jwt:Issuer"]).Returns("LibraryManagement");
        _configMock.Setup(x => x["Jwt:Audience"]).Returns("LibraryManagementClient");

        _tokenService = new TokenService(_configMock.Object, _refreshTokenRepoMock.Object);
    }

    [Fact]
    public void GenerateJwtToken_ValidUserAndRoles_ReturnsTokenAndExpiry()
    {
        var user = new AppUser
        {
            Id = 1,
            UserName = "admin",
            Email = "admin@library.com",
            FullName = "Admin User"
        };
        var roles = new List<string> { "Administrator" };

        var (token, expiresAt) = _tokenService.GenerateJwtToken(user, roles, "test-jti");

        token.Should().NotBeNullOrEmpty();
        expiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(15), TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void GenerateJwtToken_ContainsCorrectClaims()
    {
        var user = new AppUser
        {
            Id = 42,
            UserName = "librarian1",
            Email = "librarian1@library.com",
            FullName = "Alice Johnson"
        };
        var roles = new List<string> { "Librarian" };

        var (token, _) = _tokenService.GenerateJwtToken(user, roles, "test-jti-123");

        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier && c.Value == "42");
        jwt.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Name && c.Value == "librarian1");
        jwt.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Email && c.Value == "librarian1@library.com");
        jwt.Claims.Should().Contain(c => c.Type == "fullName" && c.Value == "Alice Johnson");
        jwt.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "Librarian");
        jwt.Claims.Should().Contain(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti && c.Value == "test-jti-123");
    }

    [Fact]
    public void GenerateJwtToken_MultipleRoles_AllRolesInClaims()
    {
        var user = new AppUser { Id = 1, UserName = "admin", Email = "admin@test.com", FullName = "Admin" };
        var roles = new List<string> { "Administrator", "Librarian" };

        var (token, _) = _tokenService.GenerateJwtToken(user, roles, "jti-1");

        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value)
            .Should().BeEquivalentTo(new[] { "Administrator", "Librarian" });
    }

    [Fact]
    public async Task GenerateRefreshToken_ReturnsUnusedToken()
    {
        var userId = 1;
        var jwtId = "test-jti";

        var result = await _tokenService.GenerateRefreshToken(userId, jwtId);

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.AppUserId.Should().Be(userId);
        result.JwtId.Should().Be(jwtId);
        result.IsUsed.Should().BeFalse();
        result.IsRevoked.Should().BeFalse();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        result.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromSeconds(10));
    }

    [Fact]
    public async Task GenerateRefreshToken_CallsAddAsync()
    {
        var result = await _tokenService.GenerateRefreshToken(1, "jti-test");

        _refreshTokenRepoMock.Verify(r => r.AddAsync(It.Is<RefreshToken>(rt =>
            rt.AppUserId == 1 && rt.JwtId == "jti-test" && !rt.IsUsed && !rt.IsRevoked)), Times.Once);
    }

    [Fact]
    public async Task GenerateRefreshToken_GeneratesValidBase64Token()
    {
        var result = await _tokenService.GenerateRefreshToken(1, "jti");

        var tokenBytes = Convert.FromBase64String(result.Token);
        tokenBytes.Length.Should().Be(64);
    }
}

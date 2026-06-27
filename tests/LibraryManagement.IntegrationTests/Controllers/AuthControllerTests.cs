using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithTokens()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "admin",
            password = "Password123"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<AuthResponseDto>(JsonOptions);
        content.Should().NotBeNull();
        content!.Token.Should().NotBeNullOrEmpty();
        content.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "admin",
            password = "wrongpassword"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_NonExistentUser_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "nonexistent",
            password = "Password123"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_ValidToken_ReturnsNewTokens()
    {
        var client = _factory.CreateClient();

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "admin",
            password = "Password123"
        });
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>(JsonOptions);

        var response = await client.PostAsJsonAsync("/api/auth/refresh", new
        {
            refreshToken = loginResult!.RefreshToken
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<AuthResponseDto>(JsonOptions);
        content.Should().NotBeNull();
        content!.Token.Should().NotBeNullOrEmpty();
        content.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Refresh_ExpiredToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/refresh", new
        {
            refreshToken = "invalid-refresh-token-that-does-not-exist"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Revoke_ValidToken_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "admin",
            password = "Password123"
        });
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>(JsonOptions);

        var authClient = _factory.CreateAuthenticatedClient("Administrator");

        var response = await authClient.PostAsJsonAsync("/api/auth/revoke", new
        {
            refreshToken = loginResult!.RefreshToken
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

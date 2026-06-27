using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace LibraryManagement.IntegrationTests.Controllers;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public UsersControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateAuthenticatedClient("Administrator");
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<object>>(JsonOptions);
        list.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_Existing_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/users/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>(JsonOptions);
        user.Should().NotBeNull();
        user!["username"].ToString().Should().Be("admin");
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/users/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_Valid_ReturnsCreated()
    {
        var dto = new
        {
            username = "newuser",
            email = "newuser@library.com",
            password = "Password123",
            fullName = "New User",
            role = "Staff"
        };

        var response = await _client.PostAsJsonAsync("/api/users", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_DuplicateUsername_ReturnsConflict()
    {
        var dto = new
        {
            username = "admin",
            email = "admin2@library.com",
            password = "Password123",
            fullName = "Admin Duplicate",
            role = "Staff"
        };

        var response = await _client.PostAsJsonAsync("/api/users", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Update_Existing_ReturnsNoContent()
    {
        var dto = new
        {
            fullName = "Updated Admin",
            email = "admin.updated@library.com"
        };

        var response = await _client.PutAsJsonAsync("/api/users/1", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_NonExisting_ReturnsNotFound()
    {
        var dto = new
        {
            fullName = "Nobody"
        };

        var response = await _client.PutAsJsonAsync("/api/users/9999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var response = await _client.DeleteAsync("/api/users/3");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task NonAdmin_ReturnsForbidden()
    {
        var client = _factory.CreateAuthenticatedClient("Staff");

        var response = await client.GetAsync("/api/users");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}

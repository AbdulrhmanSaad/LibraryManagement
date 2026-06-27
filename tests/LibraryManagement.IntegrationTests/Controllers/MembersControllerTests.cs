using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace LibraryManagement.IntegrationTests.Controllers;

public class MembersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public MembersControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateAuthenticatedClient("Administrator");
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/members");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<object>>(JsonOptions);
        list.Should().NotBeNull();
        list!.Count.Should().Be(3);
    }

    [Fact]
    public async Task GetById_Existing_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/members/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var member = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>(JsonOptions);
        member.Should().NotBeNull();
        member!["firstName"].ToString().Should().Be("Alice");
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/members/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_Valid_ReturnsCreated()
    {
        var dto = new
        {
            firstName = "Charlie",
            lastName = "Brown",
            email = "charlie@library.com"
        };

        var response = await _client.PostAsJsonAsync("/api/members", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_Invalid_ReturnsBadRequest()
    {
        var dto = new
        {
            firstName = "",
            lastName = "Brown",
            email = "charlie@library.com"
        };

        var response = await _client.PostAsJsonAsync("/api/members", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Update_Existing_ReturnsNoContent()
    {
        var dto = new
        {
            firstName = "AliceUpdated",
            phone = "555-0000"
        };

        var response = await _client.PutAsJsonAsync("/api/members/1", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_NonExisting_ReturnsNotFound()
    {
        var dto = new
        {
            firstName = "Nobody"
        };

        var response = await _client.PutAsJsonAsync("/api/members/9999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Admin_ReturnsNoContent()
    {
        var response = await _client.DeleteAsync("/api/members/2");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_NonAdmin_ReturnsForbidden()
    {
        var client = _factory.CreateAuthenticatedClient("Librarian");

        var response = await client.DeleteAsync("/api/members/1");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}

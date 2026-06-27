using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace LibraryManagement.IntegrationTests.Controllers;

public class AuthorsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AuthorsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateAuthenticatedClient("Administrator");
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/authors");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<object>>(JsonOptions);
        list.Should().NotBeNull();
        list!.Count.Should().Be(3);
    }

    [Fact]
    public async Task GetById_Existing_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/authors/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var author = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>(JsonOptions);
        author.Should().NotBeNull();
        author!["lastName"].ToString().Should().Be("Rowling");
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/authors/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_Valid_ReturnsCreated()
    {
        var dto = new
        {
            firstName = "Mark",
            lastName = "Twain"
        };

        var response = await _client.PostAsJsonAsync("/api/authors", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_Existing_ReturnsNoContent()
    {
        var dto = new
        {
            firstName = "Joanne",
            lastName = "Rowling"
        };

        var response = await _client.PutAsJsonAsync("/api/authors/1", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_NonExisting_ReturnsNotFound()
    {
        var dto = new
        {
            firstName = "Nobody"
        };

        var response = await _client.PutAsJsonAsync("/api/authors/9999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Existing_ReturnsNoContent()
    {
        var response = await _client.DeleteAsync("/api/authors/2");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_NonAdmin_ReturnsForbidden()
    {
        var client = _factory.CreateAuthenticatedClient("Librarian");

        var response = await client.DeleteAsync("/api/authors/1");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}

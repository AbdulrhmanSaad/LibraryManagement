using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace LibraryManagement.IntegrationTests.Controllers;

public class PublishersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public PublishersControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateAuthenticatedClient("Administrator");
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/publishers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<object>>(JsonOptions);
        list.Should().NotBeNull();
        list!.Count.Should().Be(1);
    }

    [Fact]
    public async Task GetById_Existing_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/publishers/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var publisher = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>(JsonOptions);
        publisher.Should().NotBeNull();
        publisher!["name"].ToString().Should().Be("Penguin Books");
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/publishers/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_Valid_ReturnsCreated()
    {
        var dto = new
        {
            name = "Macmillan"
        };

        var response = await _client.PostAsJsonAsync("/api/publishers", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_Existing_ReturnsNoContent()
    {
        var dto = new
        {
            name = "Penguin Books Updated",
            email = "contact@penguin.com"
        };

        var response = await _client.PutAsJsonAsync("/api/publishers/1", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_NonExisting_ReturnsNotFound()
    {
        var dto = new
        {
            name = "Ghost Publisher"
        };

        var response = await _client.PutAsJsonAsync("/api/publishers/9999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Existing_ReturnsNoContent()
    {
        var response = await _client.DeleteAsync("/api/publishers/2");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_NonAdmin_ReturnsForbidden()
    {
        var client = _factory.CreateAuthenticatedClient("Librarian");

        var response = await client.DeleteAsync("/api/publishers/1");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}

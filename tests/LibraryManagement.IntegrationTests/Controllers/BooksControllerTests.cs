using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.IntegrationTests.Controllers;

public class BooksControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public BooksControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateAuthenticatedClient("Administrator");
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithList()
    {
        var response = await _client.GetAsync("/api/books");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookDto>>(JsonOptions);
        books.Should().NotBeNull();
        books!.Count.Should().Be(3);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/books/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var book = await response.Content.ReadFromJsonAsync<BookDto>(JsonOptions);
        book.Should().NotBeNull();
        book!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/books/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByStatus_ValidStatus_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/books/status/In");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookDto>>(JsonOptions);
        books.Should().NotBeNull();
        books!.All(b => b.Status == "In").Should().BeTrue();
    }

    [Fact]
    public async Task GetByStatus_InvalidStatus_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/books/status/Unknown");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>(JsonOptions);
        error.Should().ContainKey("message");
        error!["message"].Should().Be("Status must be 'In' or 'Out'");
    }

    [Fact]
    public async Task SearchByTitle_ReturnsFiltered()
    {
        var response = await _client.GetAsync("/api/books/search?title=Harry");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookDto>>(JsonOptions);
        books.Should().NotBeNull();
        books!.Count.Should().Be(1);
        books[0].Title.Should().Be("Harry Potter");
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreated()
    {
        var client = _factory.CreateAuthenticatedClient("Librarian");

        var dto = new
        {
            isbn = "978-0-00-000000-1",
            title = "New Test Book",
            authorIds = new[] { 1 },
            categoryIds = new[] { 1 },
            publisherId = 1,
            language = "English"
        };

        var response = await client.PostAsJsonAsync("/api/books", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_DuplicateISBN_ReturnsConflict()
    {
        var client = _factory.CreateAuthenticatedClient("Librarian");

        var dto = new
        {
            isbn = "978-0-7475-3269-9",
            title = "Duplicate ISBN Book",
            authorIds = new[] { 1 },
            categoryIds = new[] { 1 },
            language = "English"
        };

        var response = await client.PostAsJsonAsync("/api/books", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>(JsonOptions);
        error.Should().ContainKey("message");
        error!["message"].Should().Be("A book with this ISBN already exists");
    }

    [Fact]
    public async Task Create_InvalidData_ReturnsBadRequest()
    {
        var client = _factory.CreateAuthenticatedClient("Librarian");

        var dto = new
        {
            isbn = "",
            title = "Test Book",
            language = "English"
        };

        var response = await client.PostAsJsonAsync("/api/books", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_StaffRole_ReturnsForbidden()
    {
        var client = _factory.CreateAuthenticatedClient("Staff");

        var dto = new
        {
            isbn = "978-0-00-000000-2",
            title = "Staff Test Book",
            authorIds = new[] { 1 },
            categoryIds = new[] { 1 },
            language = "English"
        };

        var response = await client.PostAsJsonAsync("/api/books", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_ExistingId_ReturnsNoContent()
    {
        var dto = new
        {
            title = "Updated Harry Potter"
        };

        var response = await _client.PutAsJsonAsync("/api/books/1", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_NonExistingId_ReturnsNotFound()
    {
        var dto = new
        {
            title = "Non Existent"
        };

        var response = await _client.PutAsJsonAsync("/api/books/9999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_AdminRole_ReturnsNoContent()
    {
        var response = await _client.DeleteAsync("/api/books/2");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_NonAdminRole_ReturnsForbidden()
    {
        var client = _factory.CreateAuthenticatedClient("Librarian");

        var response = await client.DeleteAsync("/api/books/3");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Unauthenticated_ReturnsUnauthorized()
    {
        var anonymousClient = _factory.CreateClient();

        var response = await anonymousClient.GetAsync("/api/books");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

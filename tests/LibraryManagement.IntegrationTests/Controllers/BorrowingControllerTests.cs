using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace LibraryManagement.IntegrationTests.Controllers;

public class BorrowingControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public BorrowingControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateAuthenticatedClient("Administrator");
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/borrowing");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<object>>(JsonOptions);
        list.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_Existing_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/borrowing/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/borrowing/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetActive_ReturnsOnlyActive()
    {
        var response = await _client.GetAsync("/api/borrowing/active");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<object>>(JsonOptions);
        list.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByMember_ReturnsBorrowings()
    {
        var response = await _client.GetAsync("/api/borrowing/member/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<object>>(JsonOptions);
        list.Should().NotBeNull();
    }

    [Fact]
    public async Task Borrow_ValidRequest_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync("/api/borrowing/borrow", new
        {
            bookId = 1,
            memberId = 1,
            borrowedById = 1,
            borrowDays = 14
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Borrow_BookUnavailable_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/borrowing/borrow", new
        {
            bookId = 3,
            memberId = 1,
            borrowedById = 1,
            borrowDays = 14
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>(JsonOptions);
        error.Should().ContainKey("message");
        error!["message"].Should().Be("Book is not available for borrowing");
    }

    [Fact]
    public async Task Borrow_BookNotFound_ReturnsNotFound()
    {
        var response = await _client.PostAsJsonAsync("/api/borrowing/borrow", new
        {
            bookId = 9999,
            memberId = 1,
            borrowedById = 1,
            borrowDays = 14
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Borrow_MemberInactive_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/borrowing/borrow", new
        {
            bookId = 2,
            memberId = 2,
            borrowedById = 1,
            borrowDays = 14
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>(JsonOptions);
        error.Should().ContainKey("message");
        error!["message"].Should().Be("Member account is inactive");
    }

    [Fact]
    public async Task Return_ValidRequest_ReturnsOk()
    {
        var borrowResponse = await _client.PostAsJsonAsync("/api/borrowing/borrow", new
        {
            bookId = 2,
            memberId = 1,
            borrowedById = 1,
            borrowDays = 14
        });
        var transaction = await borrowResponse.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var transactionId = transaction.GetProperty("id").GetInt32();

        var response = await _client.PostAsJsonAsync("/api/borrowing/return", new
        {
            transactionId,
            returnedById = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Return_AlreadyReturned_ReturnsBadRequest()
    {
        var borrowResponse = await _client.PostAsJsonAsync("/api/borrowing/borrow", new
        {
            bookId = 2,
            memberId = 1,
            borrowedById = 1,
            borrowDays = 14
        });
        var transaction = await borrowResponse.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var transactionId = transaction.GetProperty("id").GetInt32();

        var firstReturn = await _client.PostAsJsonAsync("/api/borrowing/return", new
        {
            transactionId,
            returnedById = 1
        });
        firstReturn.StatusCode.Should().Be(HttpStatusCode.OK);

        var secondReturn = await _client.PostAsJsonAsync("/api/borrowing/return", new
        {
            transactionId,
            returnedById = 1
        });
        secondReturn.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Return_TransactionNotFound_ReturnsNotFound()
    {
        var response = await _client.PostAsJsonAsync("/api/borrowing/return", new
        {
            transactionId = 9999,
            returnedById = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

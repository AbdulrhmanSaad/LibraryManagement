using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using LibraryManagement.Infrastructure.Data;

namespace LibraryManagement.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"LibraryTestDb_{Guid.NewGuid()}";
    private bool _seeded;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "",
                ["SeedDatabase"] = "false"
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<LibraryDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<LibraryDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
        });
    }

    public HttpClient CreateAuthenticatedClient(string role)
    {
        var client = CreateClient();
        var token = GenerateJwtToken(role);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public new HttpClient CreateClient()
    {
        var client = base.CreateClient();
        EnsureSeeded();
        return client;
    }

    private void EnsureSeeded()
    {
        if (_seeded) return;
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
        db.Database.EnsureCreated();
        SeedData.Initialize(scope.ServiceProvider).GetAwaiter().GetResult();
        _seeded = true;
    }

    private static string GenerateJwtToken(string role)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("SuperSecretKeyForDevelopment12345678!"));
        var expiresAt = DateTime.UtcNow.AddMinutes(15);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Email, "admin@library.com"),
            new Claim("fullName", "Admin User"),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "LibraryManagement",
            audience: "LibraryManagementClient",
            claims: claims,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

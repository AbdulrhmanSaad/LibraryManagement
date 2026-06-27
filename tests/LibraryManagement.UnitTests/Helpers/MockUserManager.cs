using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.UnitTests.Helpers;

public class MockUserManager : UserManager<AppUser>
{
    public MockUserManager()
        : base(
            new Mock<IUserStore<AppUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<AppUser>>().Object,
            Array.Empty<IUserValidator<AppUser>>(),
            Array.Empty<IPasswordValidator<AppUser>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<AppUser>>>().Object)
    {
    }

    public override Task<AppUser?> FindByIdAsync(string userId) => Task.FromResult<AppUser?>(null);
    public override Task<AppUser?> FindByNameAsync(string userName) => Task.FromResult<AppUser?>(null);
    public override Task<AppUser?> FindByEmailAsync(string email) => Task.FromResult<AppUser?>(null);
    public override Task<bool> CheckPasswordAsync(AppUser user, string password) => Task.FromResult(true);
    public override Task<IList<string>> GetRolesAsync(AppUser user) => Task.FromResult<IList<string>>(new List<string>());
    public override Task<IdentityResult> CreateAsync(AppUser user, string password) => Task.FromResult(IdentityResult.Success);
    public override Task<IdentityResult> UpdateAsync(AppUser user) => Task.FromResult(IdentityResult.Success);
    public override Task<string> GeneratePasswordResetTokenAsync(AppUser user) => Task.FromResult("reset-token");
    public override Task<IdentityResult> ResetPasswordAsync(AppUser user, string token, string password) => Task.FromResult(IdentityResult.Success);
    public override Task<IdentityResult> AddToRoleAsync(AppUser user, string role) => Task.FromResult(IdentityResult.Success);
}

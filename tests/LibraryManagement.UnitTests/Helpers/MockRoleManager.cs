using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace LibraryManagement.UnitTests.Helpers;

public class MockRoleManager : RoleManager<IdentityRole<int>>
{
    public MockRoleManager()
        : base(
            new Mock<IRoleStore<IdentityRole<int>>>().Object,
            Array.Empty<IRoleValidator<IdentityRole<int>>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<ILogger<RoleManager<IdentityRole<int>>>>().Object)
    {
    }

    public override Task<bool> RoleExistsAsync(string roleName) => Task.FromResult(true);
    public override Task<IdentityResult> CreateAsync(IdentityRole<int> role) => Task.FromResult(IdentityResult.Success);
}

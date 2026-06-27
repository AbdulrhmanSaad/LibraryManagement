using MediatR;
using Microsoft.AspNetCore.Identity;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Commands.Users.Handlers;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly UserManager<AppUser> _userManager;

    public UpdateUserCommandHandler(UserManager<AppUser> userManager) => _userManager = userManager;

    public async Task Handle(UpdateUserCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString())
            ?? throw new KeyNotFoundException("User not found");

        var dto = request.User;
        if (dto.Email != null) user.Email = dto.Email;
        if (dto.FullName != null) user.FullName = dto.FullName;
        if (dto.Password != null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);
            if (!resetResult.Succeeded)
                throw new InvalidOperationException(string.Join("; ", resetResult.Errors.Select(e => e.Description)));
        }
        if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;
        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
    }
}

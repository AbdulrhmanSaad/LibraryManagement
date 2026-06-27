using MediatR;
using Microsoft.AspNetCore.Identity;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Commands.Users.Handlers;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly UserManager<AppUser> _userManager;

    public DeleteUserCommandHandler(UserManager<AppUser> userManager) => _userManager = userManager;

    public async Task Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString())
            ?? throw new KeyNotFoundException("User not found");

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
    }
}

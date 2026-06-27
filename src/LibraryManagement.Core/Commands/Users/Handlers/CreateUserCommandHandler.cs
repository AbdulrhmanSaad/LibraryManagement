using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Commands.Users.Handlers;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager, IMapper mapper)
    {
        _userManager = userManager; _roleManager = roleManager; _mapper = mapper;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var existingUser = await _userManager.FindByNameAsync(request.User.Username);
        if (existingUser != null) throw new InvalidOperationException("Username already exists");

        var existingEmail = await _userManager.FindByEmailAsync(request.User.Email);
        if (existingEmail != null) throw new InvalidOperationException("Email already exists");

        var user = _mapper.Map<AppUser>(request.User);
        user.CreatedAt = DateTime.UtcNow;

        var createResult = await _userManager.CreateAsync(user, request.User.Password);
        if (!createResult.Succeeded)
            throw new InvalidOperationException(string.Join("; ", createResult.Errors.Select(e => e.Description)));

        if (!await _roleManager.RoleExistsAsync(request.User.Role))
            await _roleManager.CreateAsync(new IdentityRole<int>(request.User.Role));

        await _userManager.AddToRoleAsync(user, request.User.Role);
        return _mapper.Map<UserDto>(user);
    }
}

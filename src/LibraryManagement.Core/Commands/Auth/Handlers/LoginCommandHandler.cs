using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Auth.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(UserManager<AppUser> userManager, IMapper mapper, ITokenService tokenService)
    {
        _userManager = userManager; _mapper = mapper; _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByNameAsync(request.Login.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Login.Password))
            throw new UnauthorizedAccessException("Invalid username or password");
        if (!user.IsActive) throw new UnauthorizedAccessException("Account is deactivated");

        var roles = await _userManager.GetRolesAsync(user);
        var jwtId = Guid.NewGuid().ToString();
        var (token, expiresAt) = _tokenService.GenerateJwtToken(user, roles, jwtId);
        var refreshToken = await _tokenService.GenerateRefreshToken(user.Id, jwtId);


        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt
        };
    }
}
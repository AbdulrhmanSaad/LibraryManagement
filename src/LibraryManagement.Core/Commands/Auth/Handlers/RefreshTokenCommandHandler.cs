using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LibraryManagement.Core.Commands.Auth;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Auth.Handlers;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepo;

    public RefreshTokenCommandHandler(UserManager<AppUser> userManager, IMapper mapper, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepo)
    {
        _userManager = userManager; _mapper = mapper; _tokenService = tokenService; _refreshTokenRepo = refreshTokenRepo;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var storedToken = await _refreshTokenRepo.GetByTokenAsync(request.RefreshToken);
        if (storedToken == null || storedToken.IsUsed || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        var user = await _userManager.FindByIdAsync(storedToken.AppUserId.ToString());
        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("User not found or deactivated");

        storedToken.IsUsed = true;
        await _refreshTokenRepo.UpdateAsync(storedToken);

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
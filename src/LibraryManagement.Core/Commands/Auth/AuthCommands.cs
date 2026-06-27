using MediatR;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Core.Commands.Auth;

public record LoginCommand(LoginDto Login) : IRequest<AuthResponseDto>;
public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponseDto>;
public record RevokeTokenCommand(string RefreshToken) : IRequest<Unit>;
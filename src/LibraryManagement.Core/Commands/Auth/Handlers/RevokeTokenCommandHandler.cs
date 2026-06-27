using MediatR;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Auth.Handlers;

public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, Unit>
{
    private readonly IRefreshTokenRepository _refreshTokenRepo;

    public RevokeTokenCommandHandler(IRefreshTokenRepository refreshTokenRepo) => _refreshTokenRepo = refreshTokenRepo;

    public async Task<Unit> Handle(RevokeTokenCommand request, CancellationToken ct)
    {
        var storedToken = await _refreshTokenRepo.GetByTokenAsync(request.RefreshToken);
        if (storedToken == null) throw new KeyNotFoundException("Refresh token not found");

        storedToken.IsRevoked = true;
        await _refreshTokenRepo.UpdateAsync(storedToken);

        return Unit.Value;
    }
}
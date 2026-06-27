using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Queries.Users.Handlers;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(UserManager<AppUser> userManager, IMapper mapper)
    {
        _userManager = userManager; _mapper = mapper;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        return user == null ? null : _mapper.Map<UserDto>(user);
    }
}

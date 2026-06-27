using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Queries.Users.Handlers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(UserManager<AppUser> userManager, IMapper mapper)
    {
        _userManager = userManager; _mapper = mapper;
    }

    public Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken ct)
    {
        var users = _userManager.Users.ToList();
        return Task.FromResult(_mapper.Map<IEnumerable<UserDto>>(users));
    }
}

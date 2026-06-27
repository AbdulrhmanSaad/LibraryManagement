using MediatR;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Core.Queries.Users;

public record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>;
public record GetUserByIdQuery(int Id) : IRequest<UserDto?>;
public record GetUserActivityLogsQuery(int UserId) : IRequest<IEnumerable<ActivityLogDto>>;

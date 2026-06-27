using MediatR;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Core.Commands.Users;

public record CreateUserCommand(CreateUserDto User) : IRequest<UserDto>;
public record UpdateUserCommand(int Id, UpdateUserDto User) : IRequest;
public record DeleteUserCommand(int Id) : IRequest;

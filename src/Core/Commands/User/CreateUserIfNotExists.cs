using MediatR;

namespace Core.Commands.User;

public record CreateUserIfNotExists(long Id, string FirstName) : IRequest<Unit>;
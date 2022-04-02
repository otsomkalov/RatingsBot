using MediatR;

namespace Core.Requests.User;

public record CreateUserIfNotExists(long Id, string FirstName) : IRequest<Unit>;
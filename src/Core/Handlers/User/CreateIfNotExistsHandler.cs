using Core.Commands.User;
using Core.Data;
using Core.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers.User;

public class CreateIfNotExistsHandler : IRequestHandler<CreateUserIfNotExists, Unit>
{
    private readonly IAppDbContext _context;
    private readonly IUserIdProvider _userIdProvider;

    public CreateIfNotExistsHandler(IUserIdProvider userIdProvider, IAppDbContext context)
    {
        _userIdProvider = userIdProvider;
        _context = context;
    }

    public async Task<Unit> Handle(CreateUserIfNotExists request, CancellationToken cancellationToken)
    {
        var (id, firstName) = request;

        if (_userIdProvider.UserExists(id))
        {
            return Unit.Value;
        }

        _userIdProvider.AddUserId(id);

        if (await _context.Users.AnyAsync(u => u.Id == id, cancellationToken))
        {
            return Unit.Value;
        }

        await _context.Users.AddAsync(new()
        {
            Id = id,
            FirstName = firstName
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
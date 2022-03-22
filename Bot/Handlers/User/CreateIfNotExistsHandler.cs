using Bot.Commands.User;
using Microsoft.EntityFrameworkCore;

namespace Bot.Handlers.User;

public class CreateIfNotExistsHandler : AsyncRequestHandler<CreateUserIfNotExists>
{
    private readonly UserIdProvider _userIdProvider;
    private readonly AppDbContext _context;

    public CreateIfNotExistsHandler(UserIdProvider userIdProvider, AppDbContext context)
    {
        _userIdProvider = userIdProvider;
        _context = context;
    }

    protected override  async Task Handle(CreateUserIfNotExists request, CancellationToken cancellationToken)
    {
        var user = request.User;

        if (_userIdProvider.UserExists(user.Id))
        {
            return;
        }

        _userIdProvider.AddUserId(user.Id);

        if (await _context.Users.AsNoTracking().AnyAsync(u => u.Id == user.Id, cancellationToken))
        {
            return;
        }

        await _context.AddAsync(new Models.User
        {
            Id = user.Id,
            FirstName = user.FirstName
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
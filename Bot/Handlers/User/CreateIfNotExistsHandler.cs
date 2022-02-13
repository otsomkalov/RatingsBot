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
        var id = request.Id;

        if (_userIdProvider.UserExists(id))
        {
            return;
        }

        _userIdProvider.AddUserId(id);

        if (await _context.Users.AsNoTracking().AnyAsync(u => u.Id == id, cancellationToken))
        {
            return;
        }

        await _context.AddAsync(new Models.User
        {
            Id = id
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
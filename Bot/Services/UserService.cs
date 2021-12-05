using Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace Bot.Services;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly UserIdProvider _userIdProvider;

    public UserService(AppDbContext context, UserIdProvider userIdProvider)
    {
        _context = context;
        _userIdProvider = userIdProvider;
    }

    public async Task CreateIfNotExistsAsync(long id)
    {
        if (_userIdProvider.UserExists(id))
        {
            return;
        }

        _userIdProvider.AddUserId(id);

        if (await _context.Users.AsNoTracking().AnyAsync(u => u.Id == id))
        {
            return;
        }

        await _context.AddAsync(new User
        {
            Id = id
        });

        await _context.SaveChangesAsync();
    }
}
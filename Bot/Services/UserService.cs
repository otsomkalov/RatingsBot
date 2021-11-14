using Microsoft.EntityFrameworkCore;
using RatingsBot.Models;

namespace RatingsBot.Services;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateIfNotExistsAsync(long id)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == id))
        {
            await _context.AddAsync(new User
            {
                Id = id
            });

            await _context.SaveChangesAsync();
        }
    }
}
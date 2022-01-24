using Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace Bot.Services;

public class PlaceService
{
    private readonly AppDbContext _context;

    public PlaceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Place>> ListAsync()
    {
        return await _context.Places.AsNoTracking()
            .ToListAsync();
    }
}
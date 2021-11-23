using Bot.Data;
using Microsoft.EntityFrameworkCore;

namespace Bot.Services;

public class RatingService
{
    private readonly AppDbContext _context;

    public RatingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task UpsertAsync(long userId, int itemId, int entityId)
    {
        var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.ItemId == itemId);

        if (rating != null)
        {
            rating.Value = entityId;

            _context.Update(rating);
        }
        else
        {
            rating = new()
            {
                ItemId = itemId,
                UserId = userId,
                Value = entityId
            };

            await _context.AddAsync(rating);
        }

        await _context.SaveChangesAsync();
    }
}
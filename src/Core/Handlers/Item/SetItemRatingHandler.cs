using Core.Data;
using Core.Requests.Item;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers.Item;

public class SetItemRatingHandler : IRequestHandler<SetItemRating, Unit>
{
    private readonly IAppDbContext _context;

    public SetItemRatingHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(SetItemRating request, CancellationToken cancellationToken)
    {
        var (itemId, userId, ratingValue) = request;

        var rating = await _context.Ratings
            .FirstOrDefaultAsync(r => r.UserId == userId && r.ItemId == itemId, cancellationToken);

        if (rating != null)
        {
            if (rating.Value == ratingValue)
            {
                return Unit.Value;
            }

            rating = rating with
            {
                Value = ratingValue
            };

            _context.Ratings.Update(rating);
        }
        else
        {
            rating = new()
            {
                ItemId = itemId,
                UserId = userId,
                Value = ratingValue
            };

            await _context.Ratings.AddAsync(rating, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
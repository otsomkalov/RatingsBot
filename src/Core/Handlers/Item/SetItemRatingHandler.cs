using Core.Commands.Item;
using Core.Data;
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
        var (userId, entityId, itemId) = request;

        var rating = await _context.Ratings
            .FirstOrDefaultAsync(r => r.UserId == userId && r.ItemId == itemId, cancellationToken);

        if (rating != null)
        {
            rating = rating with
            {
                Value = entityId.Value
            };

            _context.Ratings.Update(rating);
        }
        else
        {
            rating = new()
            {
                ItemId = itemId,
                UserId = userId,
                Value = entityId.Value
            };

            await _context.Ratings.AddAsync(rating, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
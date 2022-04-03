using Core.Data;
using Core.Requests.Item;
using MediatR;

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

        var rating = new Models.Rating
        {
            ItemId = itemId,
            UserId = userId,
            Value = ratingValue
        };

        await _context.Ratings.AddAsync(rating, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
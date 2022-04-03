using Core.Data;
using Core.Requests.Rating;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers.Rating;

public class GetRatingHandler : IRequestHandler<GetRating, Models.Rating>
{
    private readonly IAppDbContext _context;

    public GetRatingHandler(IAppDbContext context)
    {
        _context = context;
    }

    public Task<Models.Rating> Handle(GetRating request, CancellationToken cancellationToken)
    {
        var (itemId, userId) = request;

        return _context.Ratings
            .FirstOrDefaultAsync(r => r.ItemId == itemId && r.UserId == userId, cancellationToken);
    }
}
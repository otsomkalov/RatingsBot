using Core.Data;
using Core.Requests.Item;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers.Item;

public class SetItemPlaceHandler : AsyncRequestHandler<SetItemPlace>
{
    private readonly IAppDbContext _context;

    public SetItemPlaceHandler(IAppDbContext context)
    {
        _context = context;
    }

    protected override async Task Handle(SetItemPlace request, CancellationToken cancellationToken)
    {
        var (entityId, itemId) = request;

        var item = await _context.Items
            .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

        item = item with
        {
            PlaceId = entityId
        };

        _context.Items.Update(item);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
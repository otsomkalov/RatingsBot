using Core.Commands.Item;
using Core.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers.Item;

public class SetItemManufacturerHandler : AsyncRequestHandler<SetItemManufacturer>
{
    private readonly AppDbContext _context;

    public SetItemManufacturerHandler(AppDbContext context)
    {
        _context = context;
    }

    protected override async Task Handle(SetItemManufacturer request, CancellationToken cancellationToken)
    {
        var (entityId, itemId) = request;

        var item = await _context.Items
            .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

        item = item with
        {
            ManufacturerId = entityId
        };

        _context.Items.Update(item);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
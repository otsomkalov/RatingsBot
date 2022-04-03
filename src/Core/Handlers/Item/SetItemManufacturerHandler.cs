using Core.Data;
using Core.Requests.Item;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers.Item;

public class SetItemManufacturerHandler : AsyncRequestHandler<SetItemManufacturer>
{
    private readonly IAppDbContext _context;

    public SetItemManufacturerHandler(IAppDbContext context)
    {
        _context = context;
    }

    protected override async Task Handle(SetItemManufacturer request, CancellationToken cancellationToken)
    {
        var (itemId, manufacturerId) = request;

        var item = await _context.Items
            .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

        item = item with
        {
            ManufacturerId = manufacturerId
        };

        _context.Items.Update(item);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
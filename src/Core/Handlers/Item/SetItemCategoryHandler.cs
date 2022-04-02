using Core.Commands.Item;
using Core.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers.Item;

public class SetItemCategoryHandler : AsyncRequestHandler<SetItemCategory>
{
    private readonly IAppDbContext _context;

    public SetItemCategoryHandler(IAppDbContext context)
    {
        _context = context;
    }

    protected override async Task Handle(SetItemCategory request, CancellationToken cancellationToken)
    {
        var (entityId, itemId) = request;

        var item = await _context.Items
            .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

        item = item with
        {
            CategoryId = entityId
        };

        _context.Items.Update(item);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
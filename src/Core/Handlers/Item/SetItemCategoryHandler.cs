using Core.Data;
using Core.Requests.Item;
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
        var (itemId, categoryId) = request;

        var item = await _context.Items
            .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

        item = item with
        {
            CategoryId = categoryId
        };

        _context.Items.Update(item);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
using Core.Commands.Item;
using Core.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers.Item;

public class GetItemHandler : IRequestHandler<GetItem, Models.Item>
{
    private readonly IAppDbContext _context;

    public GetItemHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Models.Item> Handle(GetItem request, CancellationToken cancellationToken)
    {
        return await _context.Items
            .Include(i => i.Category)
            .Include(i => i.Manufacturer)
            .Include(i => i.Place)
            .Include(i => i.Ratings)
            .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
    }
}
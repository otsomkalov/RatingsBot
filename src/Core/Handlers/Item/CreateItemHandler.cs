using Core.Commands.Item;
using Core.Data;
using MediatR;

namespace Core.Handlers.Item;

public class CreateItemHandler : IRequestHandler<CreateItem, Models.Item>
{
    private readonly AppDbContext _context;

    public CreateItemHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Models.Item> Handle(CreateItem request, CancellationToken cancellationToken)
    {
        var newItem = new Models.Item
        {
            Name = request.Name
        };

        await _context.AddAsync(newItem, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return newItem;
    }
}
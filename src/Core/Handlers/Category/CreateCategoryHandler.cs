using Core.Commands.Category;
using Core.Data;
using MediatR;

namespace Core.Handlers.Category;

public class CreateCategoryHandler : AsyncRequestHandler<CreateCategory>
{
    private readonly AppDbContext _context;

    public CreateCategoryHandler(AppDbContext context)
    {
        _context = context;
    }

    protected override async Task Handle(CreateCategory request, CancellationToken cancellationToken)
    {
        await _context.Categories.AddAsync(new()
        {
            Name = request.Name
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
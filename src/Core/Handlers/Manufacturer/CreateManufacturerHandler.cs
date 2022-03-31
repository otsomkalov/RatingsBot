using Core.Commands.Manufacturer;
using Core.Data;
using MediatR;

namespace Core.Handlers.Manufacturer;

public class CreateManufacturerHandler : AsyncRequestHandler<CreateManufacturer>
{
    private readonly AppDbContext _context;

    public CreateManufacturerHandler(AppDbContext context)
    {
        _context = context;
    }

    protected override async Task Handle(CreateManufacturer request, CancellationToken cancellationToken)
    {
        await _context.Manufacturers.AddAsync(new()
        {
            Name = request.Name
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
using Core.Commands.Place;
using Core.Data;
using MediatR;

namespace Core.Handlers.Place;

public class CreatePlaceHandler : AsyncRequestHandler<CreatePlace>
{
    private readonly AppDbContext _context;

    public CreatePlaceHandler(AppDbContext context)
    {
        _context = context;
    }

    protected override async Task Handle(CreatePlace request, CancellationToken cancellationToken)
    {
        await _context.Places.AddAsync(new()
        {
            Name = request.Name
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
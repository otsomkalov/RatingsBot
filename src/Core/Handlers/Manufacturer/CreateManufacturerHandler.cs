using Core.Commands.Manufacturer;
using Core.Data;
using Core.Errors;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Core.Handlers.Manufacturer;

public class CreateManufacturerHandler : IRequestHandler<CreateManufacturer, Result>
{
    private readonly IAppDbContext _context;
    private readonly IValidator<CreateManufacturer> _validator;

    public CreateManufacturerHandler(IAppDbContext context, IValidator<CreateManufacturer> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result> Handle(CreateManufacturer request, CancellationToken cancellationToken)
    {
        var result = new Result();
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return result.WithErrors(validationResult.Errors.Select(e => new ValidationError(e.ErrorMessage)));
        }

        await _context.Manufacturers.AddAsync(new()
        {
            Name = request.Name
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return result;
    }
}
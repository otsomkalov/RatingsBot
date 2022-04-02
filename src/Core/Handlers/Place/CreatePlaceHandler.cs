using Core.Data;
using Core.Errors;
using Core.Requests.Place;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Core.Handlers.Place;

public class CreatePlaceHandler : IRequestHandler<CreatePlace, Result>
{
    private readonly IAppDbContext _context;
    private readonly IValidator<CreatePlace> _validator;

    public CreatePlaceHandler(IAppDbContext context, IValidator<CreatePlace> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result> Handle(CreatePlace request, CancellationToken cancellationToken)
    {
        var result = new Result();
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return result.WithErrors(validationResult.Errors.Select(e => new ValidationError(e.ErrorMessage)));
        }

        await _context.Places.AddAsync(new()
        {
            Name = request.Name
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return result;
    }
}
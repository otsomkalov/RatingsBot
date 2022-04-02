using Core.Commands.Category;
using Core.Data;
using Core.Errors;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Core.Handlers.Category;

public class CreateCategoryHandler : IRequestHandler<CreateCategory, Result>
{
    private readonly AppDbContext _context;
    private readonly IValidator<CreateCategory> _validator;

    public CreateCategoryHandler(AppDbContext context, IValidator<CreateCategory> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result> Handle(CreateCategory request, CancellationToken cancellationToken)
    {
        var result = new Result();
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return result.WithErrors(validationResult.Errors.Select(e => new ValidationError(e.ErrorMessage)));
        }

        await _context.Categories.AddAsync(new()
        {
            Name = request.Name
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return result;
    }
}
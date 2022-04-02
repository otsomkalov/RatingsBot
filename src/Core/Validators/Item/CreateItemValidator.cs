using Core.Commands.Item;
using FluentValidation;

namespace Core.Validators.Item;

public class CreateItemValidator : AbstractValidator<CreateItem>
{
    public CreateItemValidator()
    {
        RuleFor(r => r.Name).NotNull().NotEmpty();
    }
}
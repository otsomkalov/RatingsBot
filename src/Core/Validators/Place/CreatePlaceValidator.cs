using Core.Commands.Place;
using FluentValidation;

namespace Core.Validators.Place;

public class CreatePlaceValidator : AbstractValidator<CreatePlace>
{
    public CreatePlaceValidator()
    {
        RuleFor(r => r.Name).NotNull().NotEmpty();
    }
}
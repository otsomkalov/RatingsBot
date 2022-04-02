using Core.Requests.Place;
using FluentValidation;

namespace Core.Validators.Place;

public class CreatePlaceValidator : AbstractValidator<CreatePlace>
{
    public CreatePlaceValidator()
    {
        RuleFor(r => r.Name).NotNull().NotEmpty();
    }
}
using Core.Requests.Manufacturer;
using FluentValidation;

namespace Core.Validators.Manufacturer;

public class CreateManufacturerValidator : AbstractValidator<CreateManufacturer>
{
    public CreateManufacturerValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
    }
}
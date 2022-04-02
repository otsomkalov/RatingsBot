using Core.Commands.Category;
using FluentValidation;

namespace Core.Validators.Category;

public class CreateCategoryValidator : AbstractValidator<CreateCategory>
{
    public CreateCategoryValidator()
    {
        RuleFor(r => r.Name).NotNull().NotEmpty();
    }
}
using FluentValidation;
using ProductService.DTOs;

public class CreateProductValidator : AbstractValidator<ProductCreateDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Sku)
            .NotEmpty()
            .MaximumLength(50);
    }
}
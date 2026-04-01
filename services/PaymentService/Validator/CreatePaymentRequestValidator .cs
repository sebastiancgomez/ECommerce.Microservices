namespace PaymentService.Validator;

using PaymentService.DTOs;
using FluentValidation;

public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequestDto>
{
    public CreatePaymentRequestValidator()
    {
        RuleFor(x => x.OrderId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .Must(c => new[] { "USD", "COP", "EUR" }.Contains(c.ToUpper()))
            .WithMessage("Invalid currency");
    }
}

using FluentValidation;
using NotificationService.DTOs;

namespace NotificationService.Validators;

public class SendNotificationValidator : AbstractValidator<SendNotificationDto>
{
    public SendNotificationValidator()
    {
        RuleFor(x => x.Recipient)
            .NotEmpty().WithMessage("Recipient is required.")
            .EmailAddress().WithMessage("Recipient must be a valid email address.")
            .MaximumLength(200).WithMessage("Recipient cannot exceed 200 characters.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MaximumLength(500).WithMessage("Message cannot exceed 500 characters.");
    }
}
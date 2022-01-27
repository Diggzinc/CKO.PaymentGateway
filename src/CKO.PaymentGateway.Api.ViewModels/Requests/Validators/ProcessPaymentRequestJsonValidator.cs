using CKO.PaymentGateway.Models;
using FluentValidation;

namespace CKO.PaymentGateway.Api.ViewModels.Requests.Validators;

public class ProcessPaymentRequestJsonValidator : AbstractValidator<ProcessPaymentJsonRequest>
{
    public ProcessPaymentRequestJsonValidator()
    {
        RuleFor(request => request.Card)
            .NotNull()
            .SetValidator(new CardJsonRequestValidator());

        RuleFor(request => request.Charge)
            .NotNull()
            .SetValidator(new ChargeJsonRequestValidator());

        RuleFor(request => request.Description)
            .NotEmpty()
            .MaximumLength(PaymentDescription.MaximumAllowedLength);
    }
}

using CKO.PaymentGateway.Models;
using CKO.PaymentGateway.Models.Exceptions;
using FluentValidation;

namespace CKO.PaymentGateway.Api.ViewModels.Requests.Validators;

public class ChargeJsonRequestValidator : AbstractValidator<PaymentChargeJsonRequest>
{
    public ChargeJsonRequestValidator()
    {
        RuleFor(request => request.Currency)
            .NotEmpty()
            .Must(IsValidCurrency).WithMessage("'currency' provided is not supported.");

        RuleFor(request => request.Amount)
            .GreaterThanOrEqualTo(PaymentCharge.MinimumAllowedAmountExclusive)
            .Must(IsWithinPrecisionRange).WithMessage("'amount' is not within the allowed precision for the given 'currency'.");
    }

    private static bool IsWithinPrecisionRange(PaymentChargeJsonRequest request, decimal amount)
    {
        try
        {
            var currency = Currency.FromAlphabeticCode(request.Currency);
            return amount > PaymentCharge.MinimumAllowedAmountExclusive &&
                   decimal.Round(amount, currency.MinorUnit) == amount;
        }
        catch (UnsupportedCurrencyException)
        {
            return false;
        }
    }

    private static bool IsValidCurrency(string? currency)
    {
        try
        {
            if (currency == null)
            {
                return false;
            }

            _ = Currency.FromAlphabeticCode(currency);
            return true;
        }
        catch (UnsupportedCurrencyException)
        {
            return false;
        }
    }
}

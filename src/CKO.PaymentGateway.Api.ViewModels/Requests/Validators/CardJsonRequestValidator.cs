using System.Text.RegularExpressions;
using CKO.PaymentGateway.Models;
using FluentValidation;

namespace CKO.PaymentGateway.Api.ViewModels.Requests.Validators;

public class CardJsonRequestValidator : AbstractValidator<CardJsonRequest>
{
    public CardJsonRequestValidator()
    {
        RuleFor(request => request.Holder)
            .NotEmpty()
            .MaximumLength(CardHolder.MaximumAllowedNameLength);

        RuleFor(request => request.Number)
            .NotEmpty()
            .Must(IsAllowedDynamicLength).WithMessage(AllowedDynamicLengthMessage)
            .Must(IsValidCardNumber).WithMessage("'number' is not in the correct format.");

        RuleFor(request => request.SecurityCode)
            .NotEmpty()
            .Length(CardSecurityCode.MinimumAllowedDigits, CardSecurityCode.MaximumAllowedDigits)
            .Matches(CardSecurityCode.AllowedPattern);

        RuleFor(request => request.ExpiryDate)
            .Must(IsValidSimplifiedExpiryDate).WithMessage("'expiryDate' provided does conform with the allowed format 'mm/yy'.");
    }

    private static string AllowedDynamicLengthMessage(CardJsonRequest request, string? number)
    {
        var numberWithoutWhitespacesLength = Regex.Replace(number ?? string.Empty, @"\s", string.Empty).Length;
        return $"'number' (without whitespaces) must be between {CardNumber.MinimumAllowedDigits} and {CardNumber.MaximumAllowedDigits} characters. You entered {numberWithoutWhitespacesLength} characters.";
    }

    private static bool IsAllowedDynamicLength(string? number)
    {
        var numberWithoutWhitespacesLength = Regex.Replace(number ?? string.Empty, @"\s", string.Empty).Length;
        return numberWithoutWhitespacesLength is >= CardNumber.MinimumAllowedDigits and <= CardNumber.MaximumAllowedDigits;
    }

    private static bool IsValidCardNumber(string? number)
    {
        var numberWithoutWhitespaces = Regex.Replace(number ?? string.Empty, @"\s", string.Empty);
        return Regex.IsMatch(numberWithoutWhitespaces, CardNumber.AllowedPattern);
    }

    private static bool IsValidSimplifiedExpiryDate(string? expiryDate)
    {
        const string MonthGroupName = "Month";
        const string YearGroupName = "Year";
        const string pattern = @$"^(?<{MonthGroupName}>[0-9]{{2}})/(?<{YearGroupName}>[0-9]{{2}})$";
        var regex = new Regex(pattern);

        if (!regex.IsMatch(expiryDate ?? string.Empty))
        {
            return false;
        }

        var match = regex.Matches(expiryDate ?? string.Empty).Single();
        int month = byte.Parse(match.Groups[MonthGroupName].Value);
        int year = byte.Parse(match.Groups[YearGroupName].Value);

        return month is >= CardExpiryDate.MinimumAllowedMonth and <= CardExpiryDate.MaximumAllowedMonth &&
               year <= CardExpiryDate.MaximumAllowedYear;
    }
}

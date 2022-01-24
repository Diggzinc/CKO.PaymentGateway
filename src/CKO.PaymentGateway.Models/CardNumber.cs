using CKO.PaymentGateway.Models.Exceptions;
using System.Text.RegularExpressions;

namespace CKO.PaymentGateway.Models;

/// <summary>
/// The <see cref="CardNumber"/> record.
/// Refers to the number portion of a card.
/// </summary>
/// <remarks>
/// The purpose of this record is mainly to counteract the Primitive Obsession code smell.
/// The validation of the card number domain idea in itself is rather lax here not taking into 
/// account vendor specific format requirements which should be validated before hand since 
/// they are too big and error prone to be captured within the context of the domain model.
/// </remarks>
public sealed record CardNumber
{
    /// <summary>
    /// Minimum allowed digits for a card number.
    /// </summary>
    public const byte MinimumAllowedDigits = 14;

    /// <summary>
    /// Maximum allowed digits for a card number.
    /// </summary>
    public const byte MaximumAllowedDigits = 19;

    /// <summary>
    /// The number allowed pattern.
    /// </summary>

    public static readonly string AllowedPattern = @$"^([0-9]{{{MinimumAllowedDigits},{MaximumAllowedDigits}}})$";

    /// <summary>
    /// The card number string value.
    /// </summary>
    public string Number { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CardNumber"/> record.
    /// </summary>
    /// <param name="number">The card number.</param>
    /// <exception cref="InvalidCardNumberException">The exception in case the card number is considered invalid.</exception>
    public CardNumber(string number)
    {
        Validate(number);
        Number = number;
    }

    /// <summary>
    /// Validates the candidate card number.
    /// </summary>
    /// <param name="number">The candidate card number.</param>
    /// <exception cref="InvalidCardNumberException">The exception in case the card number is considered invalid.</exception>
    // NOTE: Regexes are used for the sake of readability simplicity.
    private static void Validate(string number)
    {
        _ = number ?? throw new InvalidCardNumberException(number, "Provided card number cannot be NULL.");

        // removes potential whitespace from number.
        var numberWithoutWhitespaces = Regex.Replace(number, @"\s", string.Empty);

        // checks if the provided number is constituted by digits only
        // within the an allowed range.
        if (!Regex.IsMatch(numberWithoutWhitespaces, AllowedPattern))
        {
            throw new InvalidCardNumberException(
                number,
                $"Provided card number must contain only digits (stripped of whitespace) within the allowed range [{MinimumAllowedDigits},{MaximumAllowedDigits}].");
        }
    }

    public static implicit operator string(CardNumber cardNumber) => cardNumber.Number;
    public static implicit operator CardNumber(string number) => new(number);
}

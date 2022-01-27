using CKO.PaymentGateway.Models.Exceptions;
using System.Text.RegularExpressions;

namespace CKO.PaymentGateway.Models;

/// <summary>
/// The <see cref="CardSecurityCode"/> record.
/// Refers to the CCV2/CVV2 portion of a card.
/// </summary>
/// <remarks>
/// The purpose of this record is mainly to counteract the Primitive Obsession code smell.
/// </remarks>
public sealed record CardSecurityCode
{
    /// <summary>
    /// The minimum allowed digits.
    /// </summary>
    public const byte MinimumAllowedDigits = 3;

    /// <summary>
    /// The maximum allowed digits.
    /// </summary>
    public const byte MaximumAllowedDigits = 4;

    /// <summary>
    /// The security code allowed pattern.
    /// </summary>

    public static readonly string AllowedPattern = @$"^([0-9]{{{MinimumAllowedDigits},{MaximumAllowedDigits}}})$";

    /// <summary>
    /// The card security code string value.
    /// </summary>
    public string SecurityCode { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CardSecurityCode"/> record.
    /// </summary>
    /// <param name="securityCode">The candidate security code.</param>
    /// <exception cref="InvalidCardSecurityCodeException">The exception in case the card security code is considered invalid.</exception>
    public CardSecurityCode(string securityCode)
    {
        Validate(securityCode);
        SecurityCode = securityCode;
    }

    /// <summary>
    /// Validates the candidate card security code.
    /// </summary>
    /// <param name="securityCode">The candidate card security code.</param>
    /// <exception cref="InvalidCardSecurityCodeException">The exception in case the card security code is considered invalid.</exception>
    // NOTE: Regexes are used for the sake of readability simplicity.
    private static void Validate(string securityCode)
    {
        _ = securityCode ?? throw new InvalidCardSecurityCodeException(securityCode, "Provided security code cannot be NULL.");

        // checks if the provided number is constituted by digits only
        // within the an allowed range.
        if (!Regex.IsMatch(securityCode, AllowedPattern))
        {
            throw new InvalidCardSecurityCodeException(
                securityCode,
                $"Provided card number must contain only digits within the allowed range [{MinimumAllowedDigits},{MaximumAllowedDigits}].");
        }
    }

    public static implicit operator string(CardSecurityCode securityCode) => securityCode.SecurityCode;
    public static implicit operator CardSecurityCode(string securityCode) => new(securityCode);
}

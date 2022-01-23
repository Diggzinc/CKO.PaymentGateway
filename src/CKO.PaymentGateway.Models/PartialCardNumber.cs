using System.Text.RegularExpressions;
using CKO.PaymentGateway.Models.Exceptions;

namespace CKO.PaymentGateway.Models;

/// <summary>
/// The <see cref="PartialCardNumber"/> record.
/// Refers to a partial portion of a card number.
/// </summary>
/// <remarks>
/// The purpose of this record is mainly to counteract the Primitive Obsession code smell.
/// </remarks>
public sealed record PartialCardNumber
{
    /// <summary>
    /// The last 4 digits part of a card number string value.
    /// </summary>
    public string PartialNumber { get; init; }

    /// <summary>
    /// The length of the original card number.
    /// </summary>
    public byte NumberLength { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PartialCardNumber"/> record.
    /// </summary>
    /// <param name="partialNumber">The last 4 digits part of a card number string value.</param>
    /// <param name="numberLength">The length of the original card number.</param>
    /// <exception cref="InvalidPartialCardNumberException">The exception in case the partial card number is considered invalid.</exception>
    public PartialCardNumber(string partialNumber, byte numberLength)
    {
        Validate(partialNumber, numberLength);
        PartialNumber = partialNumber;
        NumberLength = numberLength;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PartialCardNumber"/> record based of a <see cref="CardNumber"/>.
    /// </summary>
    /// <param name="cardNumber">The original card number.</param>
    public PartialCardNumber(CardNumber cardNumber)
    {
        var number = cardNumber.Number.Replace(" ", string.Empty);
        var partialNumber = number.Substring(number.Length - 4, 4);
        var numberLength = (byte)number.Length;

        Validate(partialNumber, numberLength);

        PartialNumber = partialNumber;
        NumberLength = numberLength;
    }

    /// <summary>
    /// Validates the candidate partial card number along size the original card number length.
    /// </summary>
    /// <param name="partialNumber">The candidate card number.</param>
    /// <param name="numberLength">The original length of the card number.</param>
    /// <exception cref="InvalidPartialCardNumberException">The exception in case the partial card number is considered invalid.</exception>
    // NOTE: Regexes are used for the sake of readability simplicity.
    private static void Validate(string partialNumber, byte numberLength)
    {
        _ = partialNumber ?? throw new InvalidPartialCardNumberException(
                                partialNumber,
                                numberLength,
                                "Provided partial card number cannot be NULL.");

        const int minimumAllowedDigitsForLength = 14;
        const int maximumAllowedDigitsForLength = 19;
        if (numberLength < minimumAllowedDigitsForLength || numberLength > maximumAllowedDigitsForLength)
        {
            throw new InvalidPartialCardNumberException(
                partialNumber,
                numberLength,
                $"Provided partial card number length [{numberLength}] of the original card number must be within the allowed range [{minimumAllowedDigitsForLength},{maximumAllowedDigitsForLength}].");
        }

        // removes potential whitespace from the partial number.
        var numberWithoutWhitespaces = Regex.Replace(partialNumber, @"\s", string.Empty);

        // checks if the provided number is constituted by digits only
        // with exactly 4 digits.
        const int exactAmountOfDigitsForPartialNumber = 4;
        var pattern = @$"^([0-9]{{{exactAmountOfDigitsForPartialNumber}}})$";
        if (!Regex.IsMatch(numberWithoutWhitespaces, pattern))
        {
            throw new InvalidPartialCardNumberException(
                partialNumber,
                numberLength,
                $"Provided part card number must contain only digits (stripped of whitespace) with exactly [{exactAmountOfDigitsForPartialNumber}] length.");
        }
    }
}

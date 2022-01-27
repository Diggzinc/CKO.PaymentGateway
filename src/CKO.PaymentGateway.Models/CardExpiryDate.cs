using CKO.PaymentGateway.Models.Exceptions;

namespace CKO.PaymentGateway.Models;

/// <summary>
/// The <see cref="CardExpiryDate"/> record.
/// Refers to expiry date portion of a card.
/// </summary>
/// <remarks>
/// The purpose of this record is mainly to counteract the Primitive Obsession code smell.
/// The validation of the card expiry date domain idea in itself is rather lax here, not making 
/// many assumptions about the current year and the actual expiration state which due to the possible
/// nature of complexity regarding time validation, which similar to the card number should most
/// likely be handled on it's own and not by the model itself.
/// </remarks>
public sealed record CardExpiryDate
{
    /// <summary>
    /// The minimum allowed month.
    /// </summary>
    public const byte MinimumAllowedMonth = 1;

    /// <summary>
    /// The maximum allowed month.
    /// </summary>
    public const byte MaximumAllowedMonth = 12;

    /// <summary>
    /// The maximum allowed year;
    /// </summary>
    public const byte MaximumAllowedYear = 99;

    /// <summary>
    /// The <see cref="CardExpiryDate"/> Month component.
    /// </summary>
    public byte Month { get; init; }

    /// <summary>
    /// The <see cref="CardExpiryDate"/> Year component.
    /// </summary>
    public byte Year { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CardExpiryDate"/> record.
    /// </summary>
    /// <param name="month">The expiry date month component.</param>
    /// <param name="year">The expiry date year component.</param>
    /// <exception cref="InvalidCardNumberException">The exception in case the card expiry date is considered invalid.</exception>
    public CardExpiryDate(byte month, byte year)
    {
        Validate(month, year);
        Month = month;
        Year = year;
    }

    /// <summary>
    /// Validates the candidate card expiry date.
    /// </summary>
    /// <param name="month">The candidate expiry date month component.</param>
    /// <param name="year">The candidate expiry date year component.</param>
    /// <exception cref="InvalidCardExpiryDateException">The exception in case the card expiry date is considered invalid.</exception>
    private static void Validate(byte month, byte year)
    {

        // byte primitive is unsigned therefore cannot go lower than 0 for the year, which is valid.
        if (month is < MinimumAllowedMonth or > MaximumAllowedMonth ||
            year > MaximumAllowedYear)
        {
            throw new InvalidCardExpiryDateException(
                month,
                year,
                $"Provided  expiry date '{month}/{year}' is not within the valid range [{MinimumAllowedMonth}-{MaximumAllowedMonth}]/[0-{MaximumAllowedYear}].");
        }
    }
}

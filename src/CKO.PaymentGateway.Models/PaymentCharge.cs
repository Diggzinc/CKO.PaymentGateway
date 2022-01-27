using CKO.PaymentGateway.Models.Exceptions;

namespace CKO.PaymentGateway.Models;

/// <summary>
/// The <see cref="PaymentCharge"/> record.
/// Refers the charge applied for the issued payment operation.
/// </summary>
/// <remarks>
/// The purpose of this record is mainly to counteract the Primitive Obsession code smell.
/// </remarks>
public sealed record PaymentCharge
{
    /// <summary>
    /// The minimum allowed amount.
    /// </summary>
    public const decimal MinimumAllowedAmountExclusive = 0m;

    /// <summary>
    /// The payment charge amount component.
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// The payment charge currency component.
    /// </summary>
    public Currency Currency { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentCharge"/> record.
    /// </summary>
    /// <param name="amount">The amount of the charge.</param>
    /// <param name="currency">The currency of the charge.</param>
    /// <exception cref="InvalidPaymentChargeException">The exception in case the payment charge is considered invalid.</exception>
    public PaymentCharge(decimal amount, Currency currency)
    {
        Validate(amount, currency);
        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Validates the candidate payment charge.
    /// </summary>
    /// <param name="amount">The candidate amount of the charge.</param>
    /// <param name="currency">The candidate currency of the charge.</param>
    /// <exception cref="InvalidPaymentChargeException">The exception in case the payment charge is considered invalid.</exception>
    private static void Validate(decimal amount, Currency currency)
    {
        if (amount <= MinimumAllowedAmountExclusive || decimal.Round(amount, currency.MinorUnit) != amount)
        {
            throw new InvalidPaymentChargeException(
                amount,
                currency,
                $"Provided amount [{amount}] is not within the allowed range for the currency.");
        }
    }
}

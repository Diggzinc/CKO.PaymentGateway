namespace CKO.PaymentGateway.Models.Exceptions;

/// <summary>
/// The invalid payment charge exception.
/// </summary>
public sealed class InvalidPaymentChargeException : BusinessException
{
    /// <summary>
    /// Gets the invalid amount.
    /// </summary>
    public decimal InvalidAmount { get; }

    /// <summary>
    /// Gets the currency.
    /// </summary>
    public Currency Currency { get; }


    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPaymentChargeException"/> class.
    /// </summary>
    /// <param name="invalidAmount">The invalid amount.</param>
    /// <param name="currency">The currency.</param>
    /// <param name="message">The formatted reason message.</param>
    public InvalidPaymentChargeException(decimal invalidAmount, Currency currency, string message) : base(message)
    {
        InvalidAmount = invalidAmount;
        Currency = currency;
    }
}

namespace CKO.PaymentGateway.Models.Exceptions;

/// <summary>
/// The invalid card number exception.
/// </summary>
public sealed class InvalidCardNumberException : BusinessException
{
    /// <summary>
    /// Gets the invalid card number.
    /// </summary>
    public string? InvalidCardNumber { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidCardNumberException"/> class.
    /// </summary>
    /// <param name="invalidCardNumber">The invalid card number.</param>
    /// <param name="message">The formatted reason message.</param>
    public InvalidCardNumberException(string? invalidCardNumber, string message) : base(message)
    {
        InvalidCardNumber = invalidCardNumber;
    }
}

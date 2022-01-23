namespace CKO.PaymentGateway.Models.Exceptions;

/// <summary>
/// The invalid card number exception.
/// </summary>
public class InvalidCardNumberException : Exception
{
    /// <summary>
    /// Gets the invalid card number.
    /// </summary>
    /// <value>A string with the invalid card number.</value>
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

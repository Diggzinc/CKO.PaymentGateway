namespace CKO.PaymentGateway.Models.Exceptions;

/// <summary>
/// The invalid partial card number exception.
/// </summary>
public sealed class InvalidPartialCardNumberException : BusinessException
{
    /// <summary>
    /// Gets the invalid partial card number.
    /// </summary>
    public string? InvalidPartialCardNumber { get; }

    /// <summary>
    /// The invalid number length.
    /// </summary>
    public byte InvalidNumberLength { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPartialCardNumberException"/> class.
    /// </summary>
    /// <param name="invalidPartialCardNumber">The invalid card number.</param>
    /// <param name="invalidNumberLength">The invalid card number length.</param>
    /// <param name="message">The formatted reason message.</param>
    public InvalidPartialCardNumberException(string? invalidPartialCardNumber, byte invalidNumberLength, string message) : base(message)
    {
        InvalidPartialCardNumber = invalidPartialCardNumber;
        InvalidNumberLength = invalidNumberLength;
    }
}

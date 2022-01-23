namespace CKO.PaymentGateway.Models.Exceptions;

/// <summary>
/// The invalid card expiry date exception.
/// </summary>
public sealed class InvalidCardExpiryDateException : BusinessException
{
    /// <summary>
    /// Gets the invalid month.
    /// </summary>
    public byte InvalidMonth { get; }

    /// <summary>
    /// Gets the invalid month.
    /// </summary>
    public byte InvalidYear { get; }


    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidCardExpiryDateException"/> class.
    /// </summary>
    /// <param name="invalidMonth">The possibly invalid month.</param>
    /// <param name="invalidYear">The possibly invalid year.</param>
    /// <param name="message">The formatted reason message.</param>
    public InvalidCardExpiryDateException(byte invalidMonth, byte invalidYear, string message) : base(message)
    {
        InvalidMonth = invalidMonth;
        InvalidYear = invalidYear;
    }
}

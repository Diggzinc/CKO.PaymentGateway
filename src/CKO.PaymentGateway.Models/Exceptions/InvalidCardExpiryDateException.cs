namespace CKO.PaymentGateway.Models.Exceptions;

/// <summary>
/// The invalid card expiry date exception.
/// </summary>
public class InvalidCardExpiryDateException : Exception
{
    /// <summary>
    /// Gets the invalid month.
    /// </summary>
    /// <value>A byte with the possibly invalid month.</value>
    public byte InvalidMonth { get; }

    /// <summary>
    /// Gets the invalid month.
    /// </summary>
    /// <value>A byte with the possibly invalid year.</value>
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
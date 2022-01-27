namespace CKO.PaymentGateway.Models.Exceptions;

/// <summary>
/// The unsupported currency exception.
/// </summary>
public sealed class UnsupportedCurrencyException : BusinessException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedCurrencyException"/> class.
    /// </summary>
    /// <param name="message">The reason message.</param>
    public UnsupportedCurrencyException(string message) : base(message)
    {
    }
}

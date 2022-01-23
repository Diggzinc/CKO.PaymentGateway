namespace CKO.PaymentGateway.Models.Exceptions;

/// <summary>
/// The invalid card security code exception.
/// </summary>
public class InvalidCardSecurityCodeException : Exception
{
    /// <summary>
    /// Gets the invalid card security code.
    /// </summary>
    /// <value>A string with the invalid card security code.</value>
    public string? InvalidSecurityCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidCardSecurityCodeException"/> class.
    /// </summary>
    /// <param name="invalidSecurityCode">The invalid card security code.</param>
    /// <param name="message">The formatted reason message.</param>
    public InvalidCardSecurityCodeException(string? invalidSecurityCode, string message) : base(message)
    {
        InvalidSecurityCode = invalidSecurityCode;
    }
}

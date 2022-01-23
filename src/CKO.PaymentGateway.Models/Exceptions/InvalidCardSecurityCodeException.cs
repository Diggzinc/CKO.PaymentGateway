namespace CKO.PaymentGateway.Models.Exceptions;

/// <summary>
/// The invalid card security code exception.
/// </summary>
public sealed class InvalidCardSecurityCodeException : BusinessException
{
    /// <summary>
    /// Gets the invalid card security code.
    /// </summary>
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

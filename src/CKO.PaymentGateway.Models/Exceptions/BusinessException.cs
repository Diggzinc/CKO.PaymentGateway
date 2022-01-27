namespace CKO.PaymentGateway.Models.Exceptions;

/// <summary>
/// Represents a business exception.
/// </summary>
public abstract class BusinessException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessException"/> class.
    /// </summary>
    /// <param name="message">The formatted reason message.</param>
    protected BusinessException(string message) : base(message) { }
}

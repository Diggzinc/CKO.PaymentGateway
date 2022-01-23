namespace CKO.PaymentGateway.Models.Exceptions;

/// <summary>
/// The unsupported payment operation exception.
/// </summary>
public sealed class UnsupportedPaymentOperationException : BusinessException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedPaymentOperationException"/> class.
    /// </summary>
    /// <param name="message">The reason message.</param>
    public UnsupportedPaymentOperationException(string message) : base(message)
    {
    }
}

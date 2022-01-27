namespace CKO.PaymentGateway.Models.Exceptions;

/// <summary>
/// The invalid card holder exception.
/// </summary>
public sealed class InvalidCardHolderException : BusinessException
{
    /// <summary>
    /// Gets the invalid card holder name.
    /// </summary>
    public string? InvalidName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidCardHolderException"/> class.
    /// </summary>
    /// <param name="invalidName">The invalid card holder name.</param>
    /// <param name="message">The formatted reason message.</param>
    public InvalidCardHolderException(string? invalidName, string message) : base(message)
    {
        InvalidName = invalidName;
    }
}

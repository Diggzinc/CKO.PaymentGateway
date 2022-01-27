namespace CKO.PaymentGateway.Models.Exceptions;

/// <summary>
/// The invalid payment description exception.
/// </summary>
public sealed class InvalidPaymentDescriptionException : BusinessException
{
    /// <summary>
    /// Gets the invalid payment description.
    /// </summary>
    public string? InvalidDescription { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPaymentDescriptionException"/> class.
    /// </summary>
    /// <param name="invalidDescription">The invalid payment description.</param>
    /// <param name="message">The formatted reason message.</param>
    public InvalidPaymentDescriptionException(string? invalidDescription, string message) : base(message)
    {
        InvalidDescription = invalidDescription;
    }
}

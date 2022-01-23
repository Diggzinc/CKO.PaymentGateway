using CKO.PaymentGateway.Models.Exceptions;

namespace CKO.PaymentGateway.Models;

/// <summary>
/// The <see cref="PaymentDescription"/> record.
/// Refers to the description of the payment.
/// </summary>
/// <remarks>
/// The purpose of this record is mainly to counteract the Primitive Obsession code smell.
/// The validation is fairly lax only allowing a maximum off 255 characters.
/// </remarks>
public sealed record PaymentDescription
{
    /// <summary>
    /// The description of the payment.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentDescription"/> record.
    /// </summary>
    /// <param name="description">The payment description.</param>
    /// <exception cref="InvalidPaymentDescriptionException">The exception in case the payment description is considered invalid.</exception>
    public PaymentDescription(string description)
    {
        Validate(description);
        Description = description;
    }

    /// <summary>
    /// Validates the candidate payment description.
    /// </summary>
    /// <param name="amount">The candidate payment description.</param>
    /// <exception cref="InvalidPaymentDescriptionException">The exception in case the payment description is considered invalid.</exception>
    private static void Validate(string description)
    {
        const int maximumAllowedLength = 255;
        if (string.IsNullOrWhiteSpace(description) || description.Length > maximumAllowedLength)
        {
            throw new InvalidPaymentDescriptionException(
                description,
                $"Invalid payment description provided: {description}.");
        }
    }

    public static implicit operator string(PaymentDescription paymentDescription) => paymentDescription.Description;
    public static implicit operator PaymentDescription(string description) => new(description);
}

using CKO.PaymentGateway.Models.Exceptions;

namespace CKO.PaymentGateway.Models;

/// <summary>
/// The <see cref="CardHolder"/> record.
/// Refers to the holder name portion of a card.
/// </summary>
/// <remarks>
/// The purpose of this record is mainly to counteract the Primitive Obsession code smell.
/// The validation of the card holder name domain idea in itself is rather lax here, working under 
/// the assumption that a name must exist at the very least and be under 255 characters.
/// Due to the nature of complexity regarding name validation it should not be handled by the model itself.
/// </remarks>
public sealed record CardHolder
{
    /// <summary>
    /// Maximum allowed name length for the CardHolder.
    /// </summary>
    public const int MaximumAllowedNameLength = 255;

    /// <summary>
    /// String representation of the card holders name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CardHolder"/> record.
    /// </summary>
    /// <param name="name">The card holders name.</param>
    /// <exception cref="InvalidCardHolderException">The exception in case the card holders name is considered invalid.</exception>
    public CardHolder(string name)
    {
        Validate(name);
        Name = name;
    }

    /// <summary>
    /// Validates the candidate card holders name.
    /// </summary>
    /// <param name="name">The candidate card holders name</param>
    /// <exception cref="InvalidCardHolderException">The exception in case the card holders name is considered invalid.</exception>
    private static void Validate(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidCardHolderException(name, "Provided card holder's name cannot be NULL or whitespace.");
        }

        if (name.Length > MaximumAllowedNameLength)
        {
            throw new InvalidCardHolderException(name, $"Provided card holder's name cannot be exceed [{MaximumAllowedNameLength}] characters.");
        }
    }

    public static implicit operator string(CardHolder cardHolder) => cardHolder.Name;
    public static implicit operator CardHolder(string name) => new(name);
}

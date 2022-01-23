namespace CKO.PaymentGateway.Models;

/// <summary>
/// The <see cref="Card"/> record.
/// Refers the card to used in order to issue a payment operation.
/// </summary>
/// <param name="Number">The card number.</param>
/// <param name="SecurityCode">The card security code.</param>
/// <param name="ExpiryDate">The card expiry date.</param>
/// <param name="Holder">The card holder.</param>
public sealed record Card(
    CardNumber Number,
    CardSecurityCode SecurityCode,
    CardExpiryDate ExpiryDate,
    CardHolder Holder);

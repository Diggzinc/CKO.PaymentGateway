namespace CKO.PaymentGateway.Models;

/// <summary>
/// The <see cref="Payment"/> record.
/// Refers the card to used in order to issue a payment operation.
/// </summary>
/// <param name="Card">The card used for the payment.</param>
/// <param name="Charge">The charge applied to the card for the payment.</param>
public record Payment(
    Guid Id,
    Card Card,
    PaymentCharge Charge);

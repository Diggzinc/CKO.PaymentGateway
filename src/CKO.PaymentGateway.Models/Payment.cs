namespace CKO.PaymentGateway.Models;

/// <summary>
/// The <see cref="Payment"/> record.
/// Refers the card to used in order to issue a payment operation.
/// </summary>
/// <param name="Id">The id of the operation.</param>
/// <param name="PartialCardNumber">The partial representation of the card number used.</param>
/// <param name="CardHolder">The holder of the used card.</param>
/// <param name="CardExpiryDate">The expiry date of the used card.</param>
/// <param name="Charge">The charge applied to the card for the payment.</param>
/// <param name="Records"></param>
public sealed record Payment(
    Guid Id,
    PartialCardNumber PartialCardNumber,
    CardHolder CardHolder,
    CardExpiryDate CardExpiryDate,
    PaymentCharge Charge,
    PaymentDescription Description,
    IEnumerable<PaymentOperationRecord> Records);

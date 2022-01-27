namespace CKO.PaymentGateway.Models;

/// <summary>
/// The generic representation of a payment operation, that might include additional meta-data.
/// </summary>
/// <param name="Id">Id of the payment operation.</param>
/// <param name="Timestamp">The time of the operation.</param>
/// <param name="Operation">The payment operation.</param>
/// <param name="MetaData">Key/Value MetaData associated with the operation.</param>
public record PaymentOperationRecord(
        Guid Id,
        DateTimeOffset Timestamp,
        PaymentOperation Operation,
        IReadOnlyDictionary<string, string> MetaData);

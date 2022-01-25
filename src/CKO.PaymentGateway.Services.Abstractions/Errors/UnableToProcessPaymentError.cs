namespace CKO.PaymentGateway.Services.Abstractions.Errors;

/// <summary>
/// Signals that the corresponding payment was not able to be completely processed.
/// </summary>
/// <param name="PaymentId">The id of the unprocessed payment.</param>
/// <param name="Reason">The internal reason.</param>
public record UnableToProcessPaymentError(Guid PaymentId, string Reason) : PaymentServiceError;

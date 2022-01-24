namespace CKO.PaymentGateway.Services.Abstractions.Errors;

/// <summary>
/// Signals that the corresponding payment was not able to be completely processed.
/// </summary>
/// <param name="PaymentId">The id of the unprocessed payment.</param>
public record UnableToProcessPaymentError(Guid PaymentId) : PaymentServiceError;

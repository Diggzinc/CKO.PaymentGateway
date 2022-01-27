namespace CKO.PaymentGateway.Services.Abstractions.Errors;

/// <summary>
/// Signals that the corresponding payment for the operation was not found by the service.
/// </summary>
/// <param name="PaymentId">The id without a corresponding payment.</param>
public record PaymentNotFoundError(Guid PaymentId) : PaymentServiceError;

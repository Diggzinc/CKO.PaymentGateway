namespace AcquiringBank.Api.Client;

/// <summary>
/// The acquiring bank client.
/// </summary>
/// <remarks>
/// Assume that this was published as a NuGet package by the acquiring bank entity for convenience sake.
/// Also assume that the acquiring bank didn't follow good practices, but we have to live with that for now.
/// examples:
///     - parameters not typed
///     - primitive obsession code smell
///     - business exception results as exceptions;
/// </remarks>
public interface IAcquiringBankClient
{
    Task<Guid> IssuePaymentAsync(
        Guid merchantId,
        string iin,
        string ccv,
        string holder,
        int month,
        int year,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default);

    Task VerifyPaymentAsync(Guid transactionId, CancellationToken cancellationToken = default);

    Task AuthorizePaymentAsync(Guid transactionId, CancellationToken cancellationToken = default);

    Task ProcessPaymentAsync(Guid transactionId, CancellationToken cancellationToken = default);
}

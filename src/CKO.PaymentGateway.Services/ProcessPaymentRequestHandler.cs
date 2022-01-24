using AcquiringBank.Api.Client;
using AcquiringBank.Api.Client.Exceptions;
using CKO.PaymentGateway.Models;
using CKO.PaymentGateway.Services.Abstractions.Errors;
using CKO.PaymentGateway.Services.Abstractions.Requests;
using CKO.PaymentGateway.Services.Abstractions.Responses;
using MediatR;
using OneOf;

namespace CKO.PaymentGateway.Services;

public class ProcessPaymentRequestHandler
    : IRequestHandler<ProcessPaymentRequest, OneOf<PaymentServiceError, ProcessPaymentResponse>>
{
    private readonly IAcquiringBankClient _acquiringBankClient;

    public ProcessPaymentRequestHandler(IAcquiringBankClient acquiringBankClient)
    {
        _acquiringBankClient = acquiringBankClient;
    }

    public async Task<OneOf<PaymentServiceError, ProcessPaymentResponse>> Handle(
        ProcessPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var paymentId = Guid.NewGuid();
        (Guid merchantId, Card card, PaymentCharge charge, PaymentDescription description) = request;

        try
        {
            var transactionId = await IssuePaymentAsync(merchantId, card, charge, cancellationToken);
            await VerifyPaymentAsync(transactionId, cancellationToken);
            await AuthorizePaymentAsync(transactionId, cancellationToken);
            await ProcessPaymentAsync(transactionId, cancellationToken);

            var operations = new List<PaymentOperationRecord>();
            var payment = new Payment(
                                paymentId,
                                new PartialCardNumber(card.Number),
                                card.Holder,
                                card.ExpiryDate,
                                charge,
                                description,
                                operations);

            return new ProcessPaymentResponse(paymentId);
        }
        catch (AcquiringBankClientException exception)
        {
            return new UnableToProcessPaymentError(paymentId);
        }
        finally
        {
            // save
        }
    }

    private async Task<Guid> IssuePaymentAsync(Guid merchantId, Card card, PaymentCharge charge, CancellationToken cancellationToken)
    {
        var transactionId = await _acquiringBankClient.IssuePaymentAsync(
                                                             merchantId,
                                                             card.Number,
                                                             card.SecurityCode,
                                                             card.Holder,
                                                             card.ExpiryDate.Month,
                                                             card.ExpiryDate.Year,
                                                             charge.Amount,
                                                             charge.Currency,
                                                             cancellationToken);

        return transactionId;
        //new PaymentOperationRecord(Guid.NewGuid(),
        //        DateTimeOffset.Now,
        //        PaymentOperation.Issued,
        //        new Dictionary<string, string>
        //        {
        //            [nameof(transactionId)] = transactionId.ToString()
        //        });
    }

    private async Task VerifyPaymentAsync(Guid transactionId, CancellationToken cancellationToken)
    {
        await _acquiringBankClient.VerifyPaymentAsync(transactionId, cancellationToken);
    }

    private async Task AuthorizePaymentAsync(Guid transactionId, CancellationToken cancellationToken)
    {
        await _acquiringBankClient.AuthorizePaymentAsync(transactionId, cancellationToken);

    }

    private async Task ProcessPaymentAsync(Guid transactionId, CancellationToken cancellationToken)
    {
        await _acquiringBankClient.ProcessPaymentAsync(transactionId, cancellationToken);

    }
}

using AcquiringBank.Api.Client;
using AcquiringBank.Api.Client.Exceptions;
using AutoMapper;
using CKO.PaymentGateway.Models;
using CKO.PaymentGateway.Services.Abstractions.Errors;
using CKO.PaymentGateway.Services.Abstractions.Requests;
using CKO.PaymentGateway.Services.Abstractions.Responses;
using CKO.PaymentGateway.Services.Entities;
using MediatR;
using OneOf;

namespace CKO.PaymentGateway.Services;

public class ProcessPaymentRequestHandler
    : IRequestHandler<ProcessPaymentRequest, OneOf<PaymentServiceError, ProcessPaymentResponse>>
{
    private const string ReasonKey = "reason";
    private const string TransactionIdKey = "transactionId";

    private readonly IAcquiringBankClient _acquiringBankClient;
    private readonly PaymentGatewayContext _context;
    private readonly IMapper _mapper;

    public ProcessPaymentRequestHandler(
        IAcquiringBankClient acquiringBankClient,
        PaymentGatewayContext context,
        IMapper mapper)
    {
        _acquiringBankClient = acquiringBankClient;
        _context = context;
        _mapper = mapper;
    }

    public async Task<OneOf<PaymentServiceError, ProcessPaymentResponse>> Handle(
        ProcessPaymentRequest request,
        CancellationToken cancellationToken)
    {

        var paymentId = Guid.NewGuid();
        var transactionId = Guid.Empty;
        (Guid merchantId, Card card, PaymentCharge charge, PaymentDescription description) = request;

        try
        {
            // There is a lot of back and forth interaction with the acquiring bank.
            // For demonstration purposes we do it in just one go, and inject some errors in between
            // with the mock API.
            // We pass along the transaction id so that the state machine does not run the tasks
            // out of order, again for demonstration purposes.

            transactionId = await IssuePaymentAsync(paymentId, merchantId, card, charge, description, cancellationToken);

            transactionId = await VerifyPaymentAsync(paymentId, transactionId, cancellationToken);

            transactionId = await AuthorizePaymentAsync(paymentId, transactionId, cancellationToken);

            transactionId = await ProcessPaymentAsync(paymentId, transactionId, cancellationToken);

            return new ProcessPaymentResponse(paymentId);
        }
        catch (AcquiringBankClientException exception)
        {
            var afterRecord =
                new PaymentOperationRecord(
                    Guid.NewGuid(),
                    DateTimeOffset.Now,
                    PaymentOperation.Processed,
                    new Dictionary<string, string>
                    {
                        [TransactionIdKey] = transactionId.ToString(),
                        [ReasonKey] = exception.Reason
                    });
            var afterEntity = _mapper.Map<PaymentOperationRecordEntity>((paymentId, afterRecord));

            await _context.PaymentOperationRecords.AddAsync(afterEntity, cancellationToken);

            return new UnableToProcessPaymentError(paymentId, exception.Reason);
        }
        finally
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<Guid> IssuePaymentAsync(Guid paymentId, Guid merchantId, Card card, PaymentCharge charge, PaymentDescription description, CancellationToken cancellationToken)
    {
        var paymentEntity = _mapper.Map<PaymentEntity>((paymentId, merchantId, card, charge, description));

        await _context.Payments.AddAsync(paymentEntity, cancellationToken);

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

        var record =
            new PaymentOperationRecord(
                Guid.NewGuid(),
                DateTimeOffset.Now,
                PaymentOperation.Issued,
                new Dictionary<string, string>
                {
                    [TransactionIdKey] = transactionId.ToString()
                });

        var recordEntity = _mapper.Map<PaymentOperationRecordEntity>((paymentId, record));

        await _context.PaymentOperationRecords.AddAsync(recordEntity, cancellationToken);

        return transactionId;
    }

    private async Task<Guid> VerifyPaymentAsync(Guid paymentId, Guid transactionId, CancellationToken cancellationToken)
    {
        var beforeRecord =
            new PaymentOperationRecord(
                Guid.NewGuid(),
                DateTimeOffset.Now,
                PaymentOperation.Verifying,
                new Dictionary<string, string>
                {
                    [TransactionIdKey] = transactionId.ToString()
                });
        var beforeEntity = _mapper.Map<PaymentOperationRecordEntity>((paymentId, beforeRecord));

        await _context.PaymentOperationRecords.AddAsync(beforeEntity, cancellationToken);

        await _acquiringBankClient.VerifyPaymentAsync(transactionId, cancellationToken);

        var afterRecord =
            new PaymentOperationRecord(
                Guid.NewGuid(),
                DateTimeOffset.Now,
                PaymentOperation.Verified,
                new Dictionary<string, string>
                {
                    [TransactionIdKey] = transactionId.ToString()
                });
        var afterEntity = _mapper.Map<PaymentOperationRecordEntity>((paymentId, afterRecord));

        await _context.PaymentOperationRecords.AddAsync(afterEntity, cancellationToken);

        return transactionId;
    }

    private async Task<Guid> AuthorizePaymentAsync(Guid paymentId, Guid transactionId, CancellationToken cancellationToken)
    {
        var beforeRecord =
            new PaymentOperationRecord(
                Guid.NewGuid(),
                DateTimeOffset.Now,
                PaymentOperation.Authorizing,
                new Dictionary<string, string>
                {
                    [TransactionIdKey] = transactionId.ToString()
                });
        var beforeEntity = _mapper.Map<PaymentOperationRecordEntity>((paymentId, beforeRecord));

        await _context.PaymentOperationRecords.AddAsync(beforeEntity, cancellationToken);

        await _acquiringBankClient.AuthorizePaymentAsync(transactionId, cancellationToken);

        var afterRecord =
            new PaymentOperationRecord(
                Guid.NewGuid(),
                DateTimeOffset.Now,
                PaymentOperation.Authorized,
                new Dictionary<string, string>
                {
                    [TransactionIdKey] = transactionId.ToString()
                });
        var afterEntity = _mapper.Map<PaymentOperationRecordEntity>((paymentId, afterRecord));

        await _context.PaymentOperationRecords.AddAsync(afterEntity, cancellationToken);

        return transactionId;
    }

    private async Task<Guid> ProcessPaymentAsync(Guid paymentId, Guid transactionId, CancellationToken cancellationToken)
    {
        var beforeRecord =
            new PaymentOperationRecord(
                Guid.NewGuid(),
                DateTimeOffset.Now,
                PaymentOperation.Processing,
                new Dictionary<string, string>
                {
                    [TransactionIdKey] = transactionId.ToString()
                });
        var beforeEntity = _mapper.Map<PaymentOperationRecordEntity>((paymentId, beforeRecord));

        await _context.PaymentOperationRecords.AddAsync(beforeEntity, cancellationToken);

        await _acquiringBankClient.ProcessPaymentAsync(transactionId, cancellationToken);

        var afterRecord =
            new PaymentOperationRecord(
                Guid.NewGuid(),
                DateTimeOffset.Now,
                PaymentOperation.Processed,
                new Dictionary<string, string>
                {
                    [TransactionIdKey] = transactionId.ToString()
                });
        var afterEntity = _mapper.Map<PaymentOperationRecordEntity>((paymentId, afterRecord));

        await _context.PaymentOperationRecords.AddAsync(afterEntity, cancellationToken);

        return transactionId;
    }
}

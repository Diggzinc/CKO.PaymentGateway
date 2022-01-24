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
    public async Task<OneOf<PaymentServiceError, ProcessPaymentResponse>> Handle(
        ProcessPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var paymentId = Guid.NewGuid();
        (Guid merchantId, Card card, PaymentCharge charge, PaymentDescription description) = request;

        var response = new ProcessPaymentResponse(new Payment(
            paymentId,
            new PartialCardNumber(card.Number),
            card.Holder,
            card.ExpiryDate,
            charge,
            description,
            new List<PaymentOperationRecord>
            {
                new(Guid.NewGuid(),
                    DateTimeOffset.Now,
                    PaymentOperation.Issued,
                    new Dictionary<string, string>
                    {
                       [ "transactionId"] = Guid.NewGuid().ToString()
                    })
            }));

        return response;
    }
}

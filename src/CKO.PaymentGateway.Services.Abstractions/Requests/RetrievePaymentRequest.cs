using CKO.PaymentGateway.Services.Abstractions.Errors;
using CKO.PaymentGateway.Services.Abstractions.Responses;
using MediatR;
using OneOf;

namespace CKO.PaymentGateway.Services.Abstractions.Requests;

public record RetrievePaymentRequest(
        Guid MerchantId,
        Guid PaymentId)
    : IRequest<OneOf<PaymentServiceError, RetrievePaymentResponse>>;

using CKO.PaymentGateway.Services.Abstractions.Errors;
using CKO.PaymentGateway.Services.Abstractions.Requests;
using CKO.PaymentGateway.Services.Abstractions.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;

namespace CKO.PaymentGateway.Services;

public class ProcessPaymentRequestLoggingBehavior
    : IPipelineBehavior<ProcessPaymentRequest, OneOf<PaymentServiceError, ProcessPaymentResponse>>
{
    private readonly ILogger<ProcessPaymentRequest> _logger;

    public ProcessPaymentRequestLoggingBehavior(ILogger<ProcessPaymentRequest> logger)
    {
        _logger = logger;
    }

    public async Task<OneOf<PaymentServiceError, ProcessPaymentResponse>> Handle(
        ProcessPaymentRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<OneOf<PaymentServiceError, ProcessPaymentResponse>> next)
    {
        var result = await next();

        result.Match(
            error =>
            {
                switch (error)
                {
                    case UnableToProcessPaymentError(var paymentId, var reason):
                        _logger.LogError(
                            "Unable to process payment with id: {paymentId} because {reason}.",
                            paymentId,
                            reason);
                        break;
                }
                return Unit.Value;
            },
            response =>
            {
                _logger.LogInformation(
                    "Processed payment with id: {paymentId}",
                    response.PaymentId);
                return Unit.Value;
            });

        return result;
    }
}

using AutoMapper;
using CKO.PaymentGateway.Models;
using CKO.PaymentGateway.Services.Abstractions.Errors;
using CKO.PaymentGateway.Services.Abstractions.Requests;
using CKO.PaymentGateway.Services.Abstractions.Responses;
using CKO.PaymentGateway.Services.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace CKO.PaymentGateway.Services;

public class RetrievePaymentRequestHandler
    : IRequestHandler<RetrievePaymentRequest, OneOf<PaymentServiceError, RetrievePaymentResponse>>
{
    private readonly PaymentGatewayContext _context;
    private readonly IMapper _mapper;

    public RetrievePaymentRequestHandler(PaymentGatewayContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OneOf<PaymentServiceError, RetrievePaymentResponse>> Handle(
        RetrievePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var (merchantId, paymentId) = request;

        try
        {
            var entity = await _context
                .Payments
                    .Include(p => p.PaymentOperationRecords)
                .SingleAsync(
                    payment => payment.Id == paymentId &&
                               payment.MerchantId == merchantId,
                    cancellationToken);

            var payment = _mapper.Map<Payment>(entity);

            return new RetrievePaymentResponse(payment);
        }
        catch (InvalidOperationException)
        {
            return new PaymentNotFoundError(paymentId);
        }
    }
}

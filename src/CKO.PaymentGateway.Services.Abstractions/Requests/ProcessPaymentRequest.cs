using CKO.PaymentGateway.Models;
using CKO.PaymentGateway.Services.Abstractions.Errors;
using CKO.PaymentGateway.Services.Abstractions.Responses;
using MediatR;
using OneOf;

namespace CKO.PaymentGateway.Services.Abstractions.Requests;

/// <summary>
/// Processes a payment.
/// </summary>
public record ProcessPaymentRequest(
    Card Card,
    PaymentCharge Charge,
    PaymentDescription Description)
    : IRequest<OneOf<PaymentServiceError, ProcessPaymentResponse>>;

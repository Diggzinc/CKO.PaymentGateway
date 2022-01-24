namespace CKO.PaymentGateway.Api.ViewModels.Responses;

public readonly record struct PaymentChargeJsonResponse
{
    public string Currency { get; init; }

    public decimal Amount { get; init; }
}

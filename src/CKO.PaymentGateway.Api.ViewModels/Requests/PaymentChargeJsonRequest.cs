namespace CKO.PaymentGateway.Api.ViewModels.Requests;

public readonly record struct PaymentChargeJsonRequest
{
    public string Currency { get; init; }

    public decimal Amount { get; init; }
}

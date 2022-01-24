namespace CKO.PaymentGateway.Api.ViewModels.Requests;

public readonly record struct ProcessPaymentJsonRequest
{
    public CardJsonRequest Card { get; init; }

    public PaymentChargeJsonRequest Charge { get; init; }

    public string Description { get; init; }
}

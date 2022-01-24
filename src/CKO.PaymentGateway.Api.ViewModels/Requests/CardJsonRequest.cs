namespace CKO.PaymentGateway.Api.ViewModels.Requests;

public readonly record struct CardJsonRequest
{
    public string Number { get; init; }

    public string SecurityCode { get; init; }

    public string ExpiryDate { get; init; }

    public string Holder { get; init; }
}

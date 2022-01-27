namespace CKO.PaymentGateway.Api.ViewModels.Responses;

public readonly record struct MaskedCardJsonResponse
{
    public string Number { get; init; }

    public string ExpiryDate { get; init; }

    public string Holder { get; init; }
}

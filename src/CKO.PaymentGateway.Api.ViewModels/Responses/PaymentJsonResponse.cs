namespace CKO.PaymentGateway.Api.ViewModels.Responses;

public readonly record struct PaymentJsonResponse
{
    public Guid Id { get; init; }

    public MaskedCardJsonResponse Card { get; init; }

    public PaymentChargeJsonResponse Charge { get; init; }

    public string Description { get; init; }

    public IEnumerable<PaymentOperationRecordJsonResponse> Records { get; init; }
}

namespace CKO.PaymentGateway.Api.ViewModels.Responses;
public readonly record struct PaymentOperationRecordJsonResponse
{
    public Guid Id { get; init; }

    public string Timestamp { get; init; }

    public string Operation { get; init; }

    public IReadOnlyDictionary<string, string> MetaData { get; init; }
}

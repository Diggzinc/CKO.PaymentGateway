namespace CKO.PaymentGateway.Host.Api.Configurations;
public record PaymentGatewayApiConfiguration
{
    public string IssuerKey { get; init; }
    public string AcquiringBankApiEndpoint { get; init; }
    public string AcquiringBankApiKey { get; init; }
    public string ConnectionString { get; init; }
}

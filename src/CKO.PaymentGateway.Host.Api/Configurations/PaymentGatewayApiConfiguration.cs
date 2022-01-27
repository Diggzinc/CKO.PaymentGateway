namespace CKO.PaymentGateway.Host.Api.Configurations;

public record PaymentGatewayApiConfiguration(
    string IssuerKey,
    string AcquiringBankApiEndpoint,
    string AcquiringBankApiKey,
    string ConnectionString);

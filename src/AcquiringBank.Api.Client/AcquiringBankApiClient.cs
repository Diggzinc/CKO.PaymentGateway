using AcquiringBank.Api.Client.Exceptions;
using Flurl;
using Flurl.Http;

namespace AcquiringBank.Api.Client;

/// <summary>
/// Super dirty implementation of a client for integration sake, since it's out of scope for the project.
/// </summary>
public class AcquiringBankApiClient : IAcquiringBankClient
{
    private const string ApiKeyHeader = "X-Api-Key";
    private const string UnknownReason = "UNKNOWN";
    private readonly string _endpoint;
    private readonly string _apiKey;

    public AcquiringBankApiClient(string endpoint, string apiKey)
    {
        _endpoint = endpoint;
        _apiKey = apiKey;
    }

    public async Task<Guid> IssuePaymentAsync(Guid merchantId, string iin, string ccv, string holder, int month, int year, decimal amount, string currency, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _endpoint
                                    .AppendPathSegments("api", "v1", "merchants", merchantId, "issue")
                                    .WithHeader(ApiKeyHeader, _apiKey)
                                    .PostUrlEncodedAsync(new
                                    {
                                        iin,
                                        ccv,
                                        holder,
                                        month,
                                        year,
                                        amount,
                                        currency
                                    }, cancellationToken)
                                    .ReceiveJson();

            return new Guid(response.transactionId.ToString());
        }
        catch (FlurlHttpException exception)
        {
            var reason = await exception.GetResponseStringAsync() ?? UnknownReason;
            throw new AcquiringBankClientException(reason);
        }
    }

    public async Task VerifyPaymentAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _endpoint
                       .AppendPathSegments("api", "v1", "transactions", transactionId, "verify")
                       .WithHeader(ApiKeyHeader, _apiKey)
                       .PutAsync(cancellationToken: cancellationToken);
        }
        catch (FlurlHttpException exception)
        {
            throw new AcquiringBankClientException(UnknownReason);
        }
    }

    public async Task AuthorizePaymentAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _endpoint
                       .AppendPathSegments("api", "v1", "transactions", transactionId, "authorize")
                       .WithHeader(ApiKeyHeader, _apiKey)
                       .PutAsync(cancellationToken: cancellationToken);
        }
        catch (FlurlHttpException exception)
        {
            var reason = await exception.GetResponseStringAsync() ?? UnknownReason;
            throw new AcquiringBankClientException(reason);
        }
    }

    public async Task ProcessPaymentAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _endpoint
                       .AppendPathSegments("api", "v1", "transactions", transactionId, "process")
                       .WithHeader(ApiKeyHeader, _apiKey)
                       .PutAsync(cancellationToken: cancellationToken);
        }
        catch (FlurlHttpException exception)
        {
            var reason = await exception.GetResponseStringAsync() ?? UnknownReason;
            throw new AcquiringBankClientException(reason);
        }
    }
}

using CKO.PaymentGateway.Host.Api.Configurations;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CKO.PaymentGateway.Host.Api.HealthChecks;

public class AcquiringBankHealthCheck : IHealthCheck
{
    private readonly string _endpoint;
    private readonly ILogger<AcquiringBankHealthCheck> _logger;

    public AcquiringBankHealthCheck(
        PaymentGatewayApiConfiguration configuration,
        ILogger<AcquiringBankHealthCheck> logger)
    {
        _endpoint = configuration.AcquiringBankApiEndpoint;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _endpoint
                                    .AppendPathSegments("health")
                                    .WithTimeout(TimeSpan.FromSeconds(5))
                                    .GetAsync(cancellationToken);

            return new HealthCheckResult(
                HealthStatus.Healthy,
                description: "Acquiring bank services are operational.");
        }
        catch (FlurlHttpTimeoutException ex)
        {
            _logger.LogError(ex, "Connection to acquiring bank services timed out.");

            return new HealthCheckResult(
                context.Registration.FailureStatus,
                description: "Connection to acquiring bank services timed out.");
        }
        catch (FlurlHttpException ex) when (ex.StatusCode == StatusCodes.Status503ServiceUnavailable)
        {
            _logger.LogError(ex, "Acquiring bank services are unavailable.");

            return new HealthCheckResult(
                context.Registration.FailureStatus,
                description: "Acquiring bank services are unavailable.");
        }
        catch (FlurlHttpException ex)
        {
            _logger.LogError(ex, "Acquiring bank services are experiencing difficulties.");

            return new HealthCheckResult(
                context.Registration.FailureStatus,
                description: "Acquiring bank services are experiencing difficulties.");
        }
    }
}

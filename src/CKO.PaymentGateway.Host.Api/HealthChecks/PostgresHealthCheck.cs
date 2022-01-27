using CKO.PaymentGateway.Host.Api.Configurations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace CKO.PaymentGateway.Host.Api.HealthChecks;

public class PostgresHealthCheck : IHealthCheck
{
    private readonly string _connectionString;
    private readonly ILogger<PostgresHealthCheck> _logger;
    private const string _healthQuery = "SELECT 1;";

    public PostgresHealthCheck(
        PaymentGatewayApiConfiguration configuration,
        ILogger<PostgresHealthCheck> logger)
    {
        _connectionString = configuration.ConnectionString;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            using var command = connection.CreateCommand();
            command.CommandText = _healthQuery;

            await command.ExecuteScalarAsync(cancellationToken);

            return new HealthCheckResult(
                HealthStatus.Healthy,
                description: "Data storage instance is operational.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to connect to PostgreSQL storage instance.");

            return new HealthCheckResult(
                context.Registration.FailureStatus,
                description: "Unable to connect to storage instance.");
        }
    }
}

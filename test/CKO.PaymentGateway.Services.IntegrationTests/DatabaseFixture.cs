using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Polly;

namespace CKO.PaymentGateway.Services.IntegrationTests;
#pragma warning disable CS8618
public sealed class DatabaseFixture : IDisposable
{
    // see: docker-compose.yaml
    private const string ComposeFile = "docker-compose.yml";
    private const string DatabaseContainerName = "database";
    private const string MigrationsContainerName = "migrations";

    public const string DatabaseUser = "postgres";
    public const string DatabasePassword = "postgres";
    public const string DatabaseName = "postgres";
    public string DatabasePort { get; private set; }
    public string ConnectionString { get; private set; }

    private readonly string _stackName = Nanoid.Nanoid.Generate(
        alphabet: "1234567890abcdef",
        size: 12);

    private ICompositeService _service;
    private IDockerClient _client;

    public DatabaseFixture()
    {
        // This is not ideal (await under a constructor)
        // But I still need to find a better way to integrate with XUnit.
        SetupAndWaitForReadinessAsync().Wait();
    }

    private async Task SetupAndWaitForReadinessAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            StartStack();
            var databaseContainer = _service.Containers.First(container => container.Name == DatabaseContainerName);
            var migrationsContainer = _service.Containers.First(container => container.Name == MigrationsContainerName);

            await SetupDatabaseOptionsAsync(databaseContainer, cancellationToken);
            await WaitForReadinessAsync(databaseContainer, migrationsContainer, cancellationToken);
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    private async Task SetupDatabaseOptionsAsync(
        IContainerService databaseContainer,
        CancellationToken cancellationToken = default)
    {
        // Postgres default exposed port
        // see: docker-compose.yaml
        const string containerPort = "5432/tcp";

        // Inspects the container, then retrieves the bound host port to the default port.
        var hostPort = (await _client
                .Containers
                .InspectContainerAsync(
                    databaseContainer.Id,
                    cancellationToken))
            .NetworkSettings
            .Ports[containerPort]
            .First()
            .HostPort;

        DatabasePort = hostPort;
        ConnectionString = $"Host=localhost:{DatabasePort};Database={DatabaseName};Username={DatabaseUser};Password={DatabasePassword}";
    }

    private async Task WaitForReadinessAsync(
        IContainerService databaseContainer,
        IContainerService migrationsContainer,
        CancellationToken cancellationToken = default)
    {
        // Waits for database and migrations to be ready based on:
        // - The database container health-check result
        // - The migrations container has stopped (which means it ran)
        //
        // see: docker-compose.yaml

        const int retries = 3;

        var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(retries, count => TimeSpan.FromSeconds(count * count)); // Exponential back-off.

        await policy.ExecuteAsync(
            async () =>
            {
                // Check database
                var databaseHealthStatus =
                    (await _client
                        .Containers
                        .InspectContainerAsync(
                            databaseContainer.Id,
                            cancellationToken))
                    .State
                    .Health
                    .Status;

                switch (databaseHealthStatus)
                {
                    case "healthy":
                        break;

                    default:
                        throw new Exception($"Database container is {databaseHealthStatus}.");
                }

                // Check migrations
                var migrationsStatus =
                    (await _client
                        .Containers
                        .InspectContainerAsync(
                            migrationsContainer.Id,
                            cancellationToken))?
                    .State?
                    .Status;

                switch (migrationsStatus)
                {
                    case "exited":
                        break;

                    default:
                        throw new Exception($"Migrations container is not {migrationsStatus}.");
                }
            });
    }

    private void StartStack()
    {
        _service = new Builder()
            .UseContainer()
            .UseCompose()
            .FromFile(ComposeFile)
            .ServiceName(_stackName)
            .RemoveOrphans()
            .Build();

        _service.Start();
        var hostUri = new Uri(_service.Hosts.First().Host.AbsoluteUri);
        _client = new DockerClientConfiguration(hostUri).CreateClient();
    }

    public void Dispose()
    {
        _client?.Dispose();
        _service?.Dispose();
    }
}
#pragma warning restore CS8618

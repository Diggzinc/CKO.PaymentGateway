using CKO.PaymentGateway.Host.Api.Constants;
using CKO.PaymentGateway.Services.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CKO.PaymentGateway.Host.Api.AcceptanceTests;

internal class CustomPaymentGatewayWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable(EnvironmentVariable.IssuerKey, "your-256-bit-secret");
        Environment.SetEnvironmentVariable(EnvironmentVariable.AcquiringBankApiEndpoint, "http://localhost:3000");
        Environment.SetEnvironmentVariable(EnvironmentVariable.AcquiringBankApiKey, "some-api-key");
        Environment.SetEnvironmentVariable(EnvironmentVariable.ConnectionString, string.Empty);

        builder.ConfigureServices(services =>
        {
            var databaseDescriptor = services.Single(
                d => d.ServiceType ==
                     typeof(DbContextOptions<PaymentGatewayContext>));

            services.Remove(databaseDescriptor);

            services.AddDbContext<PaymentGatewayContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();

            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<PaymentGatewayContext>();
            var logger = scopedServices
                .GetRequiredService<ILogger<CustomPaymentGatewayWebApplicationFactory>>();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            try
            {
                InitializeDbForTests(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the " +
                                    "database with test messages. Error: {Message}", ex.Message);
            }
        });
    }

    private static void InitializeDbForTests(PaymentGatewayContext context)
    {
        context.Payments.Add(
            new()
            {
                Id = new Guid("c2fdcf74-f4d9-4a21-a7a4-c39ec65857d1"),
                MerchantId = new Guid("8005d917-3c6b-4b48-adc8-0ebe0e6dbc94"),
                PartialCardNumber = "1234",
                CardNumberLength = 16,
                CardExpiryDateMonth = 01,
                CardExpiryDateYear = 30,
                CardHolder = "John Doe",
                ChargeAmount = 100.10m,
                ChargeCurrency = "EUR",
                Description = "AMZN PURCHASE",
                PaymentOperationRecords = new()
                {
                    new()
                    {
                        Operation = "Issued",
                        Timestamp = DateTimeOffset.Now,
                        PaymentId = new Guid("c2fdcf74-f4d9-4a21-a7a4-c39ec65857d1"),
                        Id = Guid.NewGuid(),
                        MetaData = "{}"
                    }
                }
            });

        context.SaveChanges();
    }
}

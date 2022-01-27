using System;
using AcquiringBank.Api.Client;
using Microsoft.AspNetCore.Http;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace CKO.PaymentGateway.Services.IntegrationTests;

public class AcquiringBankServiceFixture : IDisposable
{
    private readonly WireMockServer _server;
    public IAcquiringBankClient Client { get; }

    public AcquiringBankServiceFixture()
    {
        _server = WireMockServer.Start();
        Client = new AcquiringBankApiClient(_server.Urls[0], "some-api-key");
    }

    public void ArrangeHappyFlow(Guid merchantId)
    {
        var transactionId = Guid.NewGuid();

        _server
            .Given(Request.Create().WithPath($"/api/v1/merchants/{merchantId}/issue").UsingPost())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(StatusCodes.Status201Created)
                    .WithBody($@"{{ ""transactionId"": ""{transactionId}"" }}"));

        _server
            .Given(Request.Create().WithPath($"/api/v1/transactions/{transactionId}/verify").UsingPut())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(StatusCodes.Status200OK));

        _server
            .Given(Request.Create().WithPath($"/api/v1/transactions/{transactionId}/authorize").UsingPut())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(StatusCodes.Status200OK));

        _server
            .Given(Request.Create().WithPath($"/api/v1/transactions/{transactionId}/process").UsingPut())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(StatusCodes.Status200OK));
    }

    public void ArrangeInsufficientFundsFailureOnAuthorize(Guid merchantId, Guid transactionId)
    {
        _server
            .Given(Request.Create().WithPath($"/api/v1/merchants/{merchantId}/issue").UsingPost())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(StatusCodes.Status201Created)
                    .WithBody($@"{{ ""transactionId"": ""{transactionId}"" }}"));

        _server
            .Given(Request.Create().WithPath($"/api/v1/transactions/{transactionId}/verify").UsingPut())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(StatusCodes.Status200OK));

        _server
            .Given(Request.Create().WithPath($"/api/v1/transactions/{transactionId}/authorize").UsingPut())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(StatusCodes.Status403Forbidden)
                    .WithBody("INSUFFICIENT_FUNDS"));
    }

    public void Dispose()
    {
        _server.Stop();
    }
}

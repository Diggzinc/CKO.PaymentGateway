using CKO.PaymentGateway.Api.ViewModels.Responses;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CKO.PaymentGateway.Host.Api.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class RetrievePaymentStepDefinitions : IClassFixture<CustomPaymentGatewayWebApplicationFactory>
{
    private readonly ScenarioContext _scenarioContext;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IFlurlClient _client;

    internal RetrievePaymentStepDefinitions(
        ScenarioContext scenarioContext,
        CustomPaymentGatewayWebApplicationFactory factory)
    {
        _scenarioContext = scenarioContext;

        _factory = factory.WithWebHostBuilder(_ => { });

        _client = new FlurlClient(_factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost/")
            }));
    }

    [Given("a merchant with name '(.*)' and merchant id '(.*)'")]
    public void Given_The_Name_And_Id_Of_The_Merchant(string merchantName, Guid merchantId)
    {
        _scenarioContext.Add("merchantName", merchantName);
        _scenarioContext.Add("merchantId", merchantId);
    }

    [Given("the authorization token '(.*)'")]
    public void GivenTheAuthorizationToken(string token)
    {
        _scenarioContext.Add("token", token);
    }

    [When("the retrieve payment is requested for id '(.*)'")]
    public async Task When_The_Retrieve_Payment_IsRequested_For_Id(Guid paymentId)
    {
        var token = _scenarioContext.Get<string>("token");

        try
        {
            var payment = await _client
                .Request("api", "v1", "payments", paymentId)
                .WithOAuthBearerToken(token)
                .GetJsonAsync<PaymentJsonResponse>();

            _scenarioContext.Add("payment", payment);
        }
        catch (FlurlHttpException ex)
        {
            _scenarioContext.Add("httpStatusCode", ex.StatusCode);
        }
    }

    [Then(@"the result should be a payment with id '(.*)'\.")]
    public void Then_The_Result_Should_Be_A_Payment_With_Id(Guid expectedPaymentId)
    {
        var payment = _scenarioContext.Get<PaymentJsonResponse>("payment");

        Assert.Equal(expectedPaymentId, payment.Id);
    }

    [Then(@"the result should be payment not found\.")]
    public void Then_The_Result_Should_Be_A_Payment_Not_Found()
    {
        var httpStatusCode = _scenarioContext.Get<int?>("httpStatusCode");

        Assert.Equal(StatusCodes.Status404NotFound, httpStatusCode);
    }

    [Then(@"the result an unauthorized response\.")]
    public void Then_The_Result_Should_An_Unauthorized_Response()
    {
        var httpStatusCode = _scenarioContext.Get<int?>("httpStatusCode");

        Assert.Equal(StatusCodes.Status401Unauthorized, httpStatusCode);
    }
}

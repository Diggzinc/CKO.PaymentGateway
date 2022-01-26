using System;
using System.Threading.Tasks;
using AutoMapper;
using CKO.PaymentGateway.Services.Abstractions.Errors;
using CKO.PaymentGateway.Services.Abstractions.Requests;
using CKO.PaymentGateway.Services.Entities;
using CKO.PaymentGateway.Services.Profiles;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CKO.PaymentGateway.Services.IntegrationTests;

[Collection(nameof(IntegrationServicesCollection))]
public class RetrievePaymentRequestHandlerTests : IDisposable
{
    private readonly PaymentGatewayContext _context;
    private readonly RetrievePaymentRequestHandler _handler;

    public RetrievePaymentRequestHandlerTests(
        DatabaseFixture databaseFixture)
    {
        var contextOptions = new DbContextOptionsBuilder<PaymentGatewayContext>()
                                    .UseNpgsql(databaseFixture.ConnectionString)
                                    .UseSnakeCaseNamingConvention()
                                    .Options;
        _context = new PaymentGatewayContext(contextOptions);

        var mapper = new MapperConfiguration(
                            cfg => cfg.AddProfile<ServiceEntitiesProfile>())
                         .CreateMapper();

        _handler = new RetrievePaymentRequestHandler(
            _context,
            mapper);
    }

    [Fact]
    public async Task Should_Return_PaymentNotFoundError_If_Payment_Does_Not_Exist()
    {
        // Arrange
        var merchantId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        var request = new RetrievePaymentRequest(merchantId, paymentId);

        // Act
        var response = await _handler.Handle(request, default);

        // Assert
        Assert.True(response.IsT0);
        Assert.IsType<PaymentNotFoundError>(response.AsT0);
    }

    [Fact]
    public async Task Should_Return_Payment_If_Payment_Does_Exists()
    {
        // Arrange
        var payment = await _context.Payments.FirstAsync();
        var merchantId = payment.MerchantId;
        var paymentId = payment.Id;
        var request = new RetrievePaymentRequest(merchantId, paymentId);

        // Act
        var response = await _handler.Handle(request, default);

        // Assert
        Assert.True(response.IsT1);
        Assert.Equal(paymentId, response.AsT1.PaymentReference.Id);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

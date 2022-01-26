using System;
using System.Threading.Tasks;
using AutoMapper;
using CKO.PaymentGateway.Models;
using CKO.PaymentGateway.Services.Abstractions.Errors;
using CKO.PaymentGateway.Services.Abstractions.Requests;
using CKO.PaymentGateway.Services.Entities;
using CKO.PaymentGateway.Services.Profiles;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CKO.PaymentGateway.Services.IntegrationTests;

[Collection(nameof(IntegrationServicesCollection))]
public class ProcessPaymentRequestHandlerTests : IDisposable
{
    private readonly PaymentGatewayContext _context;
    private readonly ProcessPaymentRequestHandler _handler;
    private readonly AcquiringBankServiceFixture _acquiringBankServices;

    public ProcessPaymentRequestHandlerTests(
        DatabaseFixture databaseFixture,
        AcquiringBankServiceFixture acquiringBankService)
    {
        var contextOptions = new DbContextOptionsBuilder<PaymentGatewayContext>()
                                    .UseNpgsql(databaseFixture.ConnectionString)
                                    .UseSnakeCaseNamingConvention()
                                    .Options;
        _context = new PaymentGatewayContext(contextOptions);
        _acquiringBankServices = acquiringBankService;
        var acquiringBank = acquiringBankService.Client;

        var mapper = new MapperConfiguration(
                            cfg => cfg.AddProfile<ServiceEntitiesProfile>())
                         .CreateMapper();

        _handler = new ProcessPaymentRequestHandler(
            acquiringBank,
            _context,
            mapper);
    }

    [Fact]
    public async Task Should_Process_Payment_When_All_Steps_Are_Successful()
    {
        // Arrange
        var merchantId = Guid.NewGuid();
        var request = new ProcessPaymentRequest(
                                        merchantId,
                                        new("1111 1111 1111 1111",
                                            "123",
                                            new(01, 30),
                                            "John Doe"),
                                        new(10.01m, Currency.EUR),
                                        "AMZN PURCHASE");

        _acquiringBankServices.ArrangeHappyFlow(merchantId);

        // Act
        var response = await _handler.Handle(request, default);

        // Assert
        Assert.True(response.IsT1);
        var paymentId = response.AsT1.PaymentId;

        var record = await _context
            .PaymentOperationRecords
            .FirstOrDefaultAsync(record => record.PaymentId == paymentId &&
                                           record.Operation == PaymentOperation.Processed.Code);
        Assert.NotNull(record);
    }

    [Fact]
    public async Task Should_Mark_Payment_As_Failed_When_Authorize_Step_Is_Unsuccessful()
    {
        // Arrange
        var merchantId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        var request = new ProcessPaymentRequest(
            merchantId,
            new("1111 1111 1111 1111",
                "123",
                new(01, 30),
                "John Doe"),
            new(10.01m, Currency.EUR),
            "AMZN PURCHASE");

        _acquiringBankServices.ArrangeInsufficientFundsFailureOnAuthorize(merchantId, transactionId);

        var response = await _handler.Handle(request, default);

        // Assert
        Assert.True(response.IsT0);
        Assert.IsType<UnableToProcessPaymentError>(response.AsT0);
        var paymentId = (response.AsT0 as UnableToProcessPaymentError)!.PaymentId;

        var record = await _context
            .PaymentOperationRecords
            .FirstOrDefaultAsync(record => record.PaymentId == paymentId &&
                                           record.Operation == PaymentOperation.Failed.Code);
        Assert.NotNull(record);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

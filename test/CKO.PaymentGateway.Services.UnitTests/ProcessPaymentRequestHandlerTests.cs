using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AcquiringBank.Api.Client;
using AutoMapper;
using CKO.PaymentGateway.Models;
using CKO.PaymentGateway.Services.Abstractions.Requests;
using CKO.PaymentGateway.Services.Entities;
using NSubstitute;
using Xunit;

namespace CKO.PaymentGateway.Services.UnitTests;

public class ProcessPaymentRequestHandlerTests
{
    [Fact]
    public async Task Should_Return_UnableToProcessPaymentError_If_AcquiringBank_Fails_To_Process_Whole_Payment()
    {
        // Arrange
        var context = Substitute.For<PaymentGatewayContext>();
        var acquiringBankClient = Substitute.For<IAcquiringBankClient>();
        var mapper = Substitute.For<IMapper>();

        var handler = new ProcessPaymentRequestHandler(acquiringBankClient, context, mapper);

        var paymentId = Guid.NewGuid();
        var merchantId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var cardExpiryDate = new CardExpiryDate(1, 22);
        var cardHolder = new CardHolder("John Doe");
        var cardSecurityCode = new CardSecurityCode("123");
        var card = new Card(
            "1111 1111 1111 1111",
            cardSecurityCode,
            cardExpiryDate,
            cardHolder);

        var charge = new PaymentCharge(100, Currency.EUR);
        var description = new PaymentDescription("some description");

        var paymentEntity = new PaymentEntity
        {
            PartialCardNumber = "1111",
            PaymentOperationRecords = new List<PaymentOperationRecordEntity>(),
            CardExpiryDateMonth = 1,
            CardExpiryDateYear = 22,
            CardHolder = "John Doe",
            CardNumberLength = 16,
            ChargeAmount = 100,
            ChargeCurrency = "EUR",
            Description = "some description",
            Id = paymentId,
            MerchantId = merchantId
        };

        acquiringBankClient
            .IssuePaymentAsync(
                merchantId,
                card.Number,
                card.SecurityCode,
                card.Holder,
                card.ExpiryDate.Month,
                card.ExpiryDate.Year,
                charge.Amount,
                charge.Currency)
            .Returns(transactionId);


        var request = new ProcessPaymentRequest(
            merchantId,
            card,
            charge,
            description);

        // Act

        await handler.Handle(request, default);

        // Assert
        await acquiringBankClient
            .IssuePaymentAsync(
                merchantId,
                card.Number,
                card.SecurityCode,
                card.Holder,
                card.ExpiryDate.Month,
                card.ExpiryDate.Year,
                charge.Amount,
                charge.Currency)
            .Received();
        await acquiringBankClient.VerifyPaymentAsync(transactionId).Received();
        await acquiringBankClient.AuthorizePaymentAsync(transactionId).Received();
        await acquiringBankClient.ProcessPaymentAsync(transactionId).Received();

    }
}

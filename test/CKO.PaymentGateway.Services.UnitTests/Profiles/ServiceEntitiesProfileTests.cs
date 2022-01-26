using System;
using System.Collections.Generic;
using AutoMapper;
using CKO.PaymentGateway.Models;
using CKO.PaymentGateway.Services.Entities;
using CKO.PaymentGateway.Services.Profiles;
using FluentAssertions;
using Xunit;

namespace CKO.PaymentGateway.Services.UnitTests.Profiles;

public class ServiceEntitiesProfileTests
{
    private readonly IMapper _mapper;

    public ServiceEntitiesProfileTests()
    {
        _mapper = new MapperConfiguration(
                configuration =>
                    configuration.AddProfile<ServiceEntitiesProfile>())
            .CreateMapper();
    }

    [Fact]
    public void Should_Map_Payment_To_PaymentJsonResponse()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var merchantId = Guid.NewGuid();
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

        var expected = new PaymentEntity
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

        // Act
        var entity = _mapper.Map<PaymentEntity>((paymentId, merchantId, card, charge, description));

        // Assert
        entity.Should().BeEquivalentTo(expected);
    }
}
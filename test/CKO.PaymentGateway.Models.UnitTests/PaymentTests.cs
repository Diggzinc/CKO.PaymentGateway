using System;
using Xunit;

namespace CKO.PaymentGateway.Models.UnitTests;

public class PaymentTests
{
    [Fact]
    public void Should_Create_Payment()
    {
        // Arrange
        var card = new Card
        (
            Number: "1111 1111 1111 1111",
            SecurityCode: "111",
            ExpiryDate: new(1, 22),
            Holder: "John Doe"
        );

        var charge = new PaymentCharge
        (
            amount: 100,
            currency: Currency.EUR
        );

        var paymentId = Guid.NewGuid();

        // Act
        var payment = new Payment
        (
            Id: paymentId,
            Card: card,
            Charge: charge
        );

        // Assert
        Assert.Equal(paymentId, payment.Id);
        Assert.Equal(card, payment.Card);
        Assert.Equal(charge, payment.Charge);
    }
}

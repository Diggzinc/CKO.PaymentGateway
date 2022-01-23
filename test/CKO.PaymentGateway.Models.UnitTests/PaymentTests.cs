using System;
using Xunit;

namespace CKO.PaymentGateway.Models.UnitTests;

public class PaymentTests
{
    [Fact]
    public void Should_Create_Payment()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var partialCardNumber = new PartialCardNumber("1111", 16);
        var cardExpiryDate = new CardExpiryDate(1, 22);
        var cardHolder = new CardHolder("John Doe");
        var charge = new PaymentCharge(100, Currency.EUR);
        var description = new PaymentDescription("some description");

        // Act
        var payment = new Payment
        (
            Id: paymentId,
            PartialCardNumber: partialCardNumber,
            CardHolder: cardHolder,
            CardExpiryDate: cardExpiryDate,
            Charge: charge,
            Description: description,
            Records: Array.Empty<PaymentOperationRecord>()
        );

        // Assert
        Assert.Equal(paymentId, payment.Id);
        Assert.Equal(partialCardNumber, payment.PartialCardNumber);
        Assert.Equal(cardHolder, payment.CardHolder);
        Assert.Equal(cardExpiryDate, payment.CardExpiryDate);
        Assert.Equal(charge, payment.Charge);
    }
}

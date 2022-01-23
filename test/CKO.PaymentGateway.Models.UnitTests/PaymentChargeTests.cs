using System.Collections.Generic;
using CKO.PaymentGateway.Models.Exceptions;
using Xunit;

namespace CKO.PaymentGateway.Models.UnitTests;
public class PaymentChargeTests
{
    public static IEnumerable<object[]> InvalidPaymentChargeTestData =>
      new List<object[]>
      {
                new object[] { 100.111m, Currency.EUR },
                new object[] { -0.111m, Currency.USD },
                new object[] { 100.111m, Currency.USD },
                new object[] { 0.111m, Currency.GBP },
                new object[] { 0.111m, Currency.CHF },
                new object[] { 100.1m, Currency.JPY },
      };

    [Theory]
    [MemberData(nameof(InvalidPaymentChargeTestData))]
    public void Should_Throw_Expected_Exception_When_PaymentCharge_Is_Invalid(decimal invalidAmount, Currency currency)
    {
        // Arrange
        // Act
        void Action() => _ = new PaymentCharge(invalidAmount, currency);

        // Assert
        var exception = Assert.Throws<InvalidPaymentChargeException>(Action);
        Assert.Equal(invalidAmount, exception.InvalidAmount);
        Assert.Equal(currency, exception.Currency);
    }

    public static IEnumerable<object[]> ValidPaymentChargeTestData =>
      new List<object[]>
      {
                new object[] { 100.11m, Currency.EUR },
                new object[] { 0.11m, Currency.USD },
                new object[] { 100.11m, Currency.USD },
                new object[] { 0.11m, Currency.GBP },
                new object[] { 0.11m, Currency.CHF },
                new object[] { 100m, Currency.JPY },
      };

    [Theory]
    [MemberData(nameof(ValidPaymentChargeTestData))]
    public void Should_Create_PaymentCharge_When_Amount_Is_Valid_For_Currency(decimal amount, Currency currency)
    {
        // Arrange
        // Act
        var paymentCharge = new PaymentCharge(amount, currency);

        // Assert
        Assert.Equal(amount, paymentCharge.Amount);
        Assert.Equal(currency, paymentCharge.Currency);
    }
}

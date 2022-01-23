using System.Collections.Generic;
using CKO.PaymentGateway.Models.Exceptions;
using Xunit;

namespace CKO.PaymentGateway.Models.UnitTests;
public class CurrencyTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("eur")]
    [InlineData("EURO")]
    public void Should_Throw_Expected_Exception_When_Provided_AlphabeticCode_Is_Unsupported(string alphabeticCode)
    {
        // Arrange
        // Act
        void Action() => _ = Currency.FromAlphabeticCode(alphabeticCode);

        // Assert
        Assert.Throws<UnsupportedCurrencyException>(Action);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void Should_Throw_Expected_Exception_When_Provided_NumericCode_Is_Unsupported(ushort numericCode)
    {
        // Arrange
        // Act
        void Action() => _ = Currency.FromNumericCode(numericCode);

        // Assert
        Assert.Throws<UnsupportedCurrencyException>(Action);
    }

    public static IEnumerable<object[]> SupportedAlphabeticCodeTestData =>
    new List<object[]>
    {
            new object[] { "EUR", Currency.EUR },
            new object[] { "USD", Currency.USD },
            new object[] { "GBP", Currency.GBP },
            new object[] { "CHF", Currency.CHF },
            new object[] { "JPY", Currency.JPY },
    };

    [Theory]
    [MemberData(nameof(SupportedAlphabeticCodeTestData))]
    public void Should_Get_Currency_From_Supported_AlphabeticCode(string alphabeticCode, Currency expectedCurrency)
    {
        // Arrange
        // Act
        var currency = Currency.FromAlphabeticCode(alphabeticCode);

        // Assert
        Assert.Equal(expectedCurrency, currency);
    }

    public static IEnumerable<object[]> SupportedNumericCodeTestData =>
    new List<object[]>
    {
            new object[] { 978, Currency.EUR },
            new object[] { 840, Currency.USD },
            new object[] { 826, Currency.GBP },
            new object[] { 756, Currency.CHF },
            new object[] { 392, Currency.JPY },
    };

    [Theory]
    [MemberData(nameof(SupportedNumericCodeTestData))]
    public void Should_Get_Currency_From_Supported_NumericCode(ushort numericCode, Currency expectedCurrency)
    {
        // Arrange
        // Act
        var currency = Currency.FromNumericCode(numericCode);

        // Assert
        Assert.Equal(expectedCurrency, currency);
    }

    [Fact]
    public void Should_Have_Default_Currency_As_EUR()
    {
        // Arrange
        var expectedCurrency = Currency.EUR;

        // Act
        var currency = new Currency();

        // Assert
        Assert.Equal(expectedCurrency, currency);
    }

    [Fact]
    public void Should_Implicitly_Convert_Currency_To_String()
    {
        // Arrange
        var expectedCurrencyString = Currency.EUR.AlphabeticCode;

        // Act
        string currencyAsString = Currency.EUR;

        // Assert
        Assert.Equal(expectedCurrencyString, currencyAsString);
    }

    [Fact]
    public void Should_Implicitly_Convert_Currency_To_UShort()
    {
        // Arrange
        ushort expectedCurrencyString = Currency.EUR.NumericCode;

        // Act
        ushort currencyAsString = Currency.EUR;

        // Assert
        Assert.Equal(expectedCurrencyString, currencyAsString);
    }
}

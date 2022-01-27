using CKO.PaymentGateway.Models.Exceptions;
using Xunit;

namespace CKO.PaymentGateway.Models.UnitTests;

public class CardExpireDateTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(13, 0)]
    [InlineData(1, 100)]
    [InlineData(1, 101)]
    public void Should_Throw_Excepted_Exception_When_CardExpireDate_Is_Invalid(byte invalidMonth, byte invalidYear)
    {
        // Arrange
        // Act
        void Action() => _ = new CardExpiryDate(invalidMonth, invalidYear);

        // Assert
        var exception = Assert.Throws<InvalidCardExpiryDateException>(Action);
        Assert.Equal(invalidMonth, exception.InvalidMonth);
        Assert.Equal(invalidYear, exception.InvalidYear);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(1, 22)]
    [InlineData(12, 22)]
    [InlineData(1, 99)]
    public void Should_Create_CardExpireDate_With_Valid_Month_Year(byte month, byte year)
    {
        // Arrange
        // Act
        var cardExpiryDate = new CardExpiryDate(month, year);

        // Assert
        Assert.Equal(month, cardExpiryDate.Month);
        Assert.Equal(year, cardExpiryDate.Year);
    }
}

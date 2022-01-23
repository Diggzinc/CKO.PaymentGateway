using CKO.PaymentGateway.Models.Exceptions;
using Xunit;

namespace CKO.PaymentGateway.Models.UnitTests;

public class PartialCardNumberTests
{
    [Theory]
    [InlineData(null, 14)]
    [InlineData("", 14)]
    [InlineData("   ", 14)]
    [InlineData("1", 14)]
    [InlineData("12", 14)]
    [InlineData("12345", 14)]
    [InlineData("ABC", 14)]
    [InlineData("A11", 14)]
    [InlineData("A111", 14)]
    [InlineData("A111A", 14)]
    [InlineData("A", 14)]
    [InlineData("WZYZ", 14)]
    [InlineData("1 23", 14)]
    [InlineData("123 23", 14)]
    [InlineData("1233", 10)]
    [InlineData("1232", 20)]
    public void Should_Throw_Excepted_Exception_When_PartialCardNumber_Is_Invalid(string invalidPartialCardNumber, byte invalidNumberLength)
    {
        // Arrange
        // Act
        void Action() => _ = new PartialCardNumber(invalidPartialCardNumber, invalidNumberLength);

        // Assert
        var exception = Assert.Throws<InvalidPartialCardNumberException>(Action);
        Assert.Equal(invalidPartialCardNumber, exception.InvalidPartialCardNumber);
        Assert.Equal(invalidNumberLength, exception.InvalidNumberLength);
    }

    [Theory]
    [InlineData("1111", 14)]
    [InlineData("1 1 1 1 ", 15)]
    [InlineData(" 1 1 1 1 ", 18)]
    [InlineData(" 1 11 1 ", 19)]
    public void Should_Create_PartialCardNumber_Within_Allowed_Range(string partialNumber, byte numberLength)
    {
        // Arrange
        // Act
        var partialCardNumber = new PartialCardNumber(partialNumber, numberLength);

        // Assert
        Assert.Equal(partialNumber, partialCardNumber.PartialNumber);
    }

    [Fact]
    public void Should_Create_PartialCardNumber_Representation_Out_Of_CardNumber()
    {
        // Arrange
        // Act
        var expectedPartialNumber = "1111";
        var expectedNumberLength = 16;
        var cardNumber = new CardNumber("1111 1111 1111 11 11");
        var partialCardNumber = new PartialCardNumber(cardNumber);

        // Assert
        Assert.Equal(expectedPartialNumber, partialCardNumber.PartialNumber);
        Assert.Equal(expectedNumberLength, partialCardNumber.NumberLength);
    }
}

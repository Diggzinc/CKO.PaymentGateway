using CKO.PaymentGateway.Models.Exceptions;
using Xunit;

namespace CKO.PaymentGateway.Models.UnitTests;

public class CardNumberTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("12345")]
    [InlineData("ABC")]
    [InlineData("A11")]
    [InlineData("A111")]
    [InlineData("A111A")]
    [InlineData("A")]
    [InlineData("WZYZ")]
    [InlineData("1 23")]
    [InlineData("123 23")]
    [InlineData("AAAA111111111111AAAA")]
    [InlineData("AAAA 1111 1111 1111 AAAA")]
    [InlineData("1111111111111111111111111111")]
    [InlineData("1111 1111 1111 1111 1111 1111 1111")]
    public void Should_Throw_Excepted_Exception_When_CardNumber_Is_Invalid(string invalidCardNumber)
    {
        // Arrange
        // Act
        void Action() => _ = new CardNumber(invalidCardNumber);

        // Assert
        var exception = Assert.Throws<InvalidCardNumberException>(Action);
        Assert.Equal(invalidCardNumber, exception.InvalidCardNumber);
    }

    [Theory]
    [InlineData("11111111111111")]
    [InlineData("1111 1111 1111 11")]
    [InlineData("1111111111111111")]
    [InlineData("1111 1111 1111 1111")]
    [InlineData("1111111111111111111")]
    [InlineData("1111 1111 1111 1111 111")]
    public void Should_Create_CardNumber_Within_Allowed_Range(string expectedCardNumber)
    {
        // Arrange
        // Act
        var cardNumber = new CardNumber(expectedCardNumber);

        // Assert
        Assert.Equal(expectedCardNumber, cardNumber.Number);
    }

    [Fact]
    public void Should_Implicitly_Convert_CardNumber_To_String()
    {
        // Arrange
        const string expectedCardNumberNumber = "1111 1111 1111 1111";
        var cardNumber = new CardNumber(expectedCardNumberNumber);

        // Act
        string cardNumberAsString = cardNumber;

        // Assert
        Assert.Equal(expectedCardNumberNumber, cardNumberAsString);
    }

    [Fact]
    public void Should_Implicitly_Convert_CardNumber_From_String()
    {
        // Arrange
        const string expectedCardNumberNumber = "1111 1111 1111 1111";
        var expectedCardNumber = new CardNumber(expectedCardNumberNumber);

        // Act
        CardNumber cardNumber = expectedCardNumberNumber;

        // Assert
        Assert.Equal(expectedCardNumber, cardNumber);
    }
}

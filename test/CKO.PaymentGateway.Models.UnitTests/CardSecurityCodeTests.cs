using CKO.PaymentGateway.Models.Exceptions;
using Xunit;

namespace CKO.PaymentGateway.Models.UnitTests;

public class CardSecurityCodeTests
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
    public void Should_Throw_Excepted_Exception_When_CardSecurityCode_Is_Invalid(string invalidSecurityCode)
    {
        // Arrange
        // Act
        void Action() => _ = new CardSecurityCode(invalidSecurityCode);

        // Assert
        var exception = Assert.Throws<InvalidCardSecurityCodeException>(Action);
        Assert.Equal(invalidSecurityCode, exception.InvalidSecurityCode);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("1234")]
    public void Should_Create_CardSecurityCode_Within_Allowed_Range(string expectedCode)
    {
        // Arrange
        // Act
        var cardSecurityCode = new CardSecurityCode(expectedCode);

        // Assert
        Assert.Equal(expectedCode, cardSecurityCode.SecurityCode);
    }

    [Fact]
    public void Should_Implicitly_Convert_CardSecurityCode_To_String()
    {
        // Arrange
        const string expectedCardSecurityCodeCode = "123";
        var cardSecurityCode = new CardSecurityCode(expectedCardSecurityCodeCode);

        // Act
        string cardSecurityCodeAsString = cardSecurityCode;

        // Assert
        Assert.Equal(expectedCardSecurityCodeCode, cardSecurityCodeAsString);
    }

    [Fact]
    public void Should_Implicitly_Convert_CardSecurityCode_From_String()
    {
        // Arrange
        const string expectedCardSecurityCodeCode = "123";
        var expectedCardSecurityCode = new CardSecurityCode(expectedCardSecurityCodeCode);

        // Act
        CardSecurityCode cardSecurityCode = expectedCardSecurityCodeCode;

        // Assert
        Assert.Equal(expectedCardSecurityCode, cardSecurityCode);
    }
}

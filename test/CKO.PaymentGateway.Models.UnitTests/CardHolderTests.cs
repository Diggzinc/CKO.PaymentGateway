using System.Linq;
using CKO.PaymentGateway.Models.Exceptions;
using Xunit;

namespace CKO.PaymentGateway.Models.UnitTests;

public class CardHolderTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Throw_Excepted_Exception_When_CardHolder_Is_Invalid_1(string invalidName)
    {
        // Arrange
        // Act
        void Action() => _ = new CardHolder(invalidName);

        // Assert
        var exception = Assert.Throws<InvalidCardHolderException>(Action);
        Assert.Equal(invalidName, exception.InvalidName);
    }

    [Fact]
    public void Should_Throw_Excepted_Exception_When_CardHolder_Is_Invalid_2()
    {
        // Arrange
        var invalidName = string.Join(string.Empty, Enumerable.Repeat("a", 256));
        // Act
        void Action() => _ = new CardHolder(invalidName);

        // Assert
        var exception = Assert.Throws<InvalidCardHolderException>(Action);
        Assert.Equal(invalidName, exception.InvalidName);
    }

    [Fact]
    public void Should_Create_Card_Holder_With_Valid_Name()
    {
        // Arrange
        const string expectedName = "John Doe";

        // Act
        var cardHolder = new CardHolder(expectedName);

        // Assert
        Assert.Equal(expectedName, cardHolder.Name);
    }

    [Fact]
    public void Should_Implicitly_Convert_CardHolder_To_String()
    {
        // Arrange
        const string expectedName = "John Doe";
        var cardHolder = new CardHolder(expectedName);

        // Act
        string cardHolderAsString = cardHolder;

        // Assert
        Assert.Equal(expectedName, cardHolderAsString);
    }

    [Fact]
    public void Should_Implicitly_Convert_CardHolder_From_String()
    {
        // Arrange
        const string expectedName = "John Doe";
        var expectedCardHolder = new CardHolder(expectedName);

        // Act
        CardHolder cardHolder = expectedName;

        // Assert
        Assert.Equal(expectedCardHolder, cardHolder);
    }
}

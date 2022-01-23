using System.Linq;
using CKO.PaymentGateway.Models.Exceptions;
using Xunit;

namespace CKO.PaymentGateway.Models.UnitTests;

public class PaymentDescriptionTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Throw_Excepted_Exception_When_PaymentDescription_Is_Invalid_1(string invalidDescription)
    {
        // Arrange
        // Act
        void Action() => _ = new PaymentDescription(invalidDescription);

        // Assert
        var exception = Assert.Throws<InvalidPaymentDescriptionException>(Action);
        Assert.Equal(invalidDescription, exception.InvalidDescription);
    }

    [Fact]
    public void Should_Throw_Excepted_Exception_When_PaymentDescription_Is_Invalid_2()
    {
        // Arrange
        var invalidDescription = string.Join(string.Empty, Enumerable.Repeat("a", 256));
        // Act
        void Action() => _ = new PaymentDescription(invalidDescription);

        // Assert
        var exception = Assert.Throws<InvalidPaymentDescriptionException>(Action);
        Assert.Equal(invalidDescription, exception.InvalidDescription);
    }

    [Fact]
    public void Should_Create_Card_Holder_With_Valid_Name()
    {
        // Arrange
        const string expectedDescription = "some description";

        // Act
        var paymentDescription = new PaymentDescription(expectedDescription);

        // Assert
        Assert.Equal(expectedDescription, paymentDescription.Description);
    }

    [Fact]
    public void Should_Implicitly_Convert_PaymentDescription_To_String()
    {
        // Arrange
        const string expectedDescription = "some description";
        var paymentDescription = new PaymentDescription(expectedDescription);

        // Act
        string paymentDescriptionAsString = paymentDescription;

        // Assert
        Assert.Equal(expectedDescription, paymentDescriptionAsString);
    }

    [Fact]
    public void Should_Implicitly_Convert_PaymentDescription_From_String()
    {
        // Arrange
        const string expectedDescription = "John Doe";
        var expectedPaymentDescription = new PaymentDescription(expectedDescription);

        // Act
        PaymentDescription paymentDescription = expectedDescription;

        // Assert
        Assert.Equal(expectedPaymentDescription, paymentDescription);
    }
}

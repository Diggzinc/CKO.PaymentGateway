using System.Collections.Generic;
using CKO.PaymentGateway.Models.Exceptions;
using Xunit;

namespace CKO.PaymentGateway.Models.UnitTests;

public class PaymentOperationTests
{
    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void Should_Throw_Expected_Exception_When_Provided_Id_Is_Unsupported(ushort id)
    {
        // Arrange
        // Act
        void Action() => _ = PaymentOperation.FromId(id);

        // Assert
        Assert.Throws<UnsupportedPaymentOperationException>(Action);
    }

    public static IEnumerable<object[]> SupportedIdsTestData =>
    new List<object[]>
    {
            new object[] { 0, PaymentOperation.Issued },
            new object[] { 1, PaymentOperation.Verifying },
            new object[] { 2, PaymentOperation.Verified },
            new object[] { 3, PaymentOperation.Authorizing },
            new object[] { 4, PaymentOperation.Authorized },
            new object[] { 5, PaymentOperation.Processing },
            new object[] { 6, PaymentOperation.Processed },
            new object[] { 7, PaymentOperation.Failed },
    };

    [Theory]
    [MemberData(nameof(SupportedIdsTestData))]
    public void Should_Get_PaymentOperation_From_Supported_Id(ushort id, PaymentOperation expectedPaymentOperation)
    {
        // Arrange
        // Act
        var paymentOperation = PaymentOperation.FromId(id);

        // Assert
        Assert.Equal(expectedPaymentOperation, paymentOperation);
    }

    public static IEnumerable<object[]> SupportedCodesTestData =>
        new List<object[]>
        {
            new object[] { "Issued", PaymentOperation.Issued },
            new object[] { "Verifying", PaymentOperation.Verifying },
            new object[] { "Verified", PaymentOperation.Verified },
            new object[] { "Authorizing", PaymentOperation.Authorizing },
            new object[] { "Authorized", PaymentOperation.Authorized },
            new object[] { "Processing", PaymentOperation.Processing },
            new object[] { "Processed", PaymentOperation.Processed },
            new object[] { "Failed", PaymentOperation.Failed },
        };

    [Theory]
    [MemberData(nameof(SupportedCodesTestData))]
    public void Should_Get_PaymentOperation_From_Supported_Code(string code, PaymentOperation expectedPaymentOperation)
    {
        // Arrange
        // Act
        var paymentOperation = PaymentOperation.FromCode(code);

        // Assert
        Assert.Equal(expectedPaymentOperation, paymentOperation);
    }

    [Fact]
    public void Should_Have_Default_PaymentOperation_As_Issued()
    {
        // Arrange
        var expectedPaymentOperation = PaymentOperation.Issued;

        // Act
        var paymentOperation = new PaymentOperation();

        // Assert
        Assert.Equal(expectedPaymentOperation, paymentOperation);
    }
}

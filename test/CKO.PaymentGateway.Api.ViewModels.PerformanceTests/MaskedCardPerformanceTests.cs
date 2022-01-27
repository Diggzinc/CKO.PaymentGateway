using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using CKO.PaymentGateway.Api.ViewModels.Profiles;
using CKO.PaymentGateway.Api.ViewModels.Responses;
using CKO.PaymentGateway.Models;

namespace CKO.PaymentGateway.Api.ViewModels.PerformanceTests;

[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
[MinColumn, MaxColumn, MedianColumn, KurtosisColumn]
[HtmlExporter]
public class MaskedCardPerformanceTests
{
    private readonly Payment _payment;

    public MaskedCardPerformanceTests()
    {
        var paymentId = Guid.NewGuid();
        var partialCardNumber = new PartialCardNumber("1111", 16);
        var cardExpiryDate = new CardExpiryDate(1, 22);
        var cardHolder = new CardHolder("John Doe");
        var charge = new PaymentCharge(100, Currency.EUR);
        var description = new PaymentDescription("some description");

        _payment = new Payment
        (
            Id: paymentId,
            PartialCardNumber: partialCardNumber,
            CardHolder: cardHolder,
            CardExpiryDate: cardExpiryDate,
            Charge: charge,
            Description: description,
            Records: Array.Empty<PaymentOperationRecord>()
        );
    }

    [Benchmark(Baseline = true)]
    public MaskedCardJsonResponse ToMaskedCardJsonResponse() => ApiViewModelsProfile.ToMaskedCardJsonResponse(_payment);

    [Benchmark]
    public MaskedCardJsonResponse BetterToMaskedCardJsonResponse() => ApiViewModelsProfile.BetterToMaskedCardJsonResponse(_payment);
}

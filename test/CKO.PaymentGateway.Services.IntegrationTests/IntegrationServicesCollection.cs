using Xunit;

namespace CKO.PaymentGateway.Services.IntegrationTests;

[CollectionDefinition(nameof(IntegrationServicesCollection))]
public class IntegrationServicesCollection
    : ICollectionFixture<DatabaseFixture>,
      ICollectionFixture<AcquiringBankServiceFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

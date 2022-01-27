using CKO.PaymentGateway.Api.ViewModels;
using CKO.PaymentGateway.Models;
using CKO.PaymentGateway.Services;
using CKO.PaymentGateway.Services.Abstractions;
using NetArchTest.Rules;
using NetArchTest.Rules.Policies;
using Xunit;
using Xunit.Abstractions;

namespace CKO.PaymentGateway.ArchitectureTests
{
    public class LayeredArchitectureTests
    {
        private readonly ITestOutputHelper _output;

        public LayeredArchitectureTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Architecture_Should_Comply_With_Layered_Architecture_Principles()
        {
            // Arrange
            var types = Types.InAssemblies(
                new[]
                {
                    typeof(PaymentGatewayServicesAnchor).Assembly,
                    typeof(ApiViewModelsAnchor).Assembly,
                    typeof(PaymentGatewayServicesAbstractionsAnchor).Assembly,
                    typeof(ModelsAnchor).Assembly,
                });

            var layeredArchitecturePolicy =
                Policy.Define(
                        "LayeredArchitecturePolicy",
                        "Policy to enforce a layered architecture.")
                    .For(types)
                    .Add(
                        definition: t =>
                           t.That()
                            .ResideInNamespace("CKO.PaymentGateway.Api.ViewModels")
                            .ShouldNot()
                            .HaveDependencyOn("CKO.PaymentGateway.Services"),
                        name: "ServiceEntitiesCannotDependOnApiViewModels",
                        description: "Service entities cannot depend on api view models.")
                    .Add(
                        definition: t =>
                            t.That()
                                .ResideInNamespace("CKO.PaymentGateway.Api.ViewModels")
                                .ShouldNot()
                                .HaveDependencyOn("CKO.PaymentGateway.Services"),
                        name: "ApiViewModelsCannotDependOnEntities",
                        description: "Api view models cannot depend on service entities.")
                    .Add(
                        definition: t =>
                            t.That()
                                .ResideInNamespace("CKO.PaymentGateway.Services.Profiles")
                                .Should()
                                .HaveDependencyOn("CKO.PaymentGateway.Models"),
                        name: "ServiceEntitiesMappersShouldDependOnDomainEntities",
                        description: "Service entities should depend on domain entities (mappers).")
                    .Add(
                        definition: t =>
                            t.That()
                                .ResideInNamespace("CKO.PaymentGateway.Api.ViewModels.Profiles")
                                .Should()
                                .HaveDependencyOn("CKO.PaymentGateway.Models"),
                        name: "ApiViewModelsMappersShouldDependOnDomainEntities",
                        description: "Api view models should depend on domain entities (mappers).");

            // Act
            var evaluation = layeredArchitecturePolicy.Evaluate();

            // Assert
            foreach (var result in evaluation.Results)
            {
                _output.WriteLine("policy: {0}", result.Name);
                _output.WriteLine("description: {0}", result.Description);
                _output.WriteLine("\tstatus {0}", result.IsSuccessful ? "pass" : "fail");
                _output.WriteLine(string.Empty);
            }
            Assert.False(evaluation.HasViolations);
        }
    }
}

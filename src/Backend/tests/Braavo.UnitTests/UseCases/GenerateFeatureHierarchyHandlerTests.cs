using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Diagrams;
using Braavo.Core.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class GenerateFeatureHierarchyHandlerTests
{
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly IFeatureRepository _featureRepo = Substitute.For<IFeatureRepository>();
    private readonly GenerateFeatureHierarchyHandler _handler;

    public GenerateFeatureHierarchyHandlerTests()
    {
        _handler = new GenerateFeatureHierarchyHandler(_productRepo, _featureRepo);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        _productRepo.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns((Product?)null);

        var result = await _handler.Handle(new GenerateFeatureHierarchyCommand(productId, userId), CancellationToken.None);

        result.Success.Should().BeFalse();
        result.MermaidCode.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ProductWrongOwner_ReturnsFailure()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var product = Product.Create("Test Product", "Desc", UserId.From(ownerId));

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        var result = await _handler.Handle(new GenerateFeatureHierarchyCommand(product.Id, otherUserId), CancellationToken.None);

        result.Success.Should().BeFalse();
        result.MermaidCode.Should().BeNull();
    }

    [Fact]
    public async Task Handle_NoFeatures_ReturnsFailureWithUnprocessableEntity()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Empty Product", "Desc", UserId.From(userId));

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<Feature>());

        var result = await _handler.Handle(new GenerateFeatureHierarchyCommand(product.Id, userId), CancellationToken.None);

        result.Success.Should().BeFalse();
        result.IsUnprocessableEntity.Should().BeTrue();
        result.MermaidCode.Should().BeNull();
    }

    [Fact]
    public async Task Handle_FeaturesAcrossMultiplePhases_ReturnsMindmapWithPhasesInOrder()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Braavo", "Desc", UserId.From(userId));

        var mvpFeature = Feature.Create(product.Id, "User Login", "Auth flow");
        // Phase is Mvp by default

        var enhancedFeature = Feature.Create(product.Id, "Dashboard", "Analytics view");
        enhancedFeature.ChangePhase(FeaturePhase.Enhanced);

        var futureFeature = Feature.Create(product.Id, "AI Insights", "ML predictions");
        futureFeature.ChangePhase(FeaturePhase.Future);

        var features = new List<Feature> { futureFeature, enhancedFeature, mvpFeature };

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(features);

        var result = await _handler.Handle(new GenerateFeatureHierarchyCommand(product.Id, userId), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.MermaidCode.Should().NotBeNull();
        result.MermaidCode!.Should().StartWith("mindmap");

        var mermaid = result.MermaidCode!;
        var mvpIndex = mermaid.IndexOf("MVP", StringComparison.Ordinal);
        var enhancedIndex = mermaid.IndexOf("Enhanced", StringComparison.Ordinal);
        var futureIndex = mermaid.IndexOf("Future", StringComparison.Ordinal);

        mvpIndex.Should().BeGreaterThan(-1, "MermaidCode should contain MVP phase heading");
        enhancedIndex.Should().BeGreaterThan(-1, "MermaidCode should contain Enhanced phase heading");
        futureIndex.Should().BeGreaterThan(-1, "MermaidCode should contain Future phase heading");

        mvpIndex.Should().BeLessThan(enhancedIndex, "MVP should appear before Enhanced");
        enhancedIndex.Should().BeLessThan(futureIndex, "Enhanced should appear before Future");
    }
}

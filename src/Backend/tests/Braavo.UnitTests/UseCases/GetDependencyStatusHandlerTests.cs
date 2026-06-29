using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Products;
using Braavo.Core.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class GetDependencyStatusHandlerTests
{
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly IPersonaRepository _personaRepo = Substitute.For<IPersonaRepository>();
    private readonly IFeatureRepository _featureRepo = Substitute.For<IFeatureRepository>();
    private readonly IUserStoryRepository _storyRepo = Substitute.For<IUserStoryRepository>();
    private readonly GetDependencyStatusHandler _handler;

    public GetDependencyStatusHandlerTests()
    {
        _handler = new GetDependencyStatusHandler(_productRepo, _personaRepo, _featureRepo, _storyRepo);
    }

    [Fact]
    public async Task Handle_EmptyProduct_ReturnsCorrectStatus()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(userId));

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<Persona>());
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<Feature>());
        _storyRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<UserStory>());

        var result = await _handler.Handle(new GetDependencyStatusQuery(product.Id, userId), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.HasPersonas.Should().BeFalse();
        result.HasFeatures.Should().BeFalse();
        result.HasStories.Should().BeFalse();
        result.CanCreateFeatures.Should().BeFalse();
        result.CanCreateStories.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_HasPersonas_CanCreateFeatures()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(userId));
        var persona = Persona.Create(product.Id, "User", "Dev");

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(new[] { persona });
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<Feature>());
        _storyRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<UserStory>());

        var result = await _handler.Handle(new GetDependencyStatusQuery(product.Id, userId), CancellationToken.None);

        result.HasPersonas.Should().BeTrue();
        result.CanCreateFeatures.Should().BeTrue();
        result.CanCreateStories.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_HasPersonasAndFeatures_CanCreateStories()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(userId));
        var persona = Persona.Create(product.Id, "User", "Dev");
        var feature = Feature.Create(product.Id, "Login", "Auth");

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(new[] { persona });
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(new[] { feature });
        _storyRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<UserStory>());

        var result = await _handler.Handle(new GetDependencyStatusQuery(product.Id, userId), CancellationToken.None);

        result.HasPersonas.Should().BeTrue();
        result.HasFeatures.Should().BeTrue();
        result.CanCreateFeatures.Should().BeTrue();
        result.CanCreateStories.Should().BeTrue();
    }
}

using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Products;
using Braavo.Core.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class GetProductHandlerTests
{
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly IPersonaRepository _personaRepo = Substitute.For<IPersonaRepository>();
    private readonly IUserStoryRepository _storyRepo = Substitute.For<IUserStoryRepository>();
    private readonly IFeatureRepository _featureRepo = Substitute.For<IFeatureRepository>();
    private readonly GetProductHandler _handler;

    public GetProductHandlerTests()
    {
        _handler = new GetProductHandler(_productRepo, _personaRepo, _storyRepo, _featureRepo);
    }

    [Fact]
    public async Task Handle_WithValidOwner_ReturnsProduct()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(userId));

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns([]);
        _storyRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns([]);
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns([]);

        var result = await _handler.Handle(new GetProductQuery(product.Id, userId), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Test");
    }

    [Fact]
    public async Task Handle_WithWrongOwner_ReturnsNull()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(ownerId));

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        var result = await _handler.Handle(new GetProductQuery(product.Id, otherUserId), CancellationToken.None);

        result.Should().BeNull();
    }
}

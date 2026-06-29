using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.UserStories;
using Braavo.Core.ValueObjects;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class CreateUserStoryHandlerTests
{
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly IUserStoryRepository _storyRepo = Substitute.For<IUserStoryRepository>();
    private readonly IFeatureRepository _featureRepo = Substitute.For<IFeatureRepository>();
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly CreateUserStoryHandler _handler;

    public CreateUserStoryHandlerTests()
    {
        _handler = new CreateUserStoryHandler(_productRepo, _storyRepo, _featureRepo, _mediator);
    }

    [Fact]
    public async Task Handle_NoFeaturesExist_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(userId));

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<Feature>());

        var command = new CreateUserStoryCommand(product.Id, userId, "As a user", "I want to login", "So I can access my account");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Be("At least one feature is required before creating user stories");
    }

    [Fact]
    public async Task Handle_FeatureExists_CreatesStory()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(userId));
        var feature = Feature.Create(product.Id, "Login", "User authentication");

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(new[] { feature });

        var command = new CreateUserStoryCommand(product.Id, userId, "As a user", "I want to login", "So I can access my account");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.StoryId.Should().NotBeEmpty();
    }
}

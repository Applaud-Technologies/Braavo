using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Features;
using Braavo.Core.ValueObjects;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class CreateFeatureHandlerTests
{
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly IFeatureRepository _featureRepo = Substitute.For<IFeatureRepository>();
    private readonly IPersonaRepository _personaRepo = Substitute.For<IPersonaRepository>();
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly CreateFeatureHandler _handler;

    public CreateFeatureHandlerTests()
    {
        _handler = new CreateFeatureHandler(_productRepo, _featureRepo, _personaRepo, _mediator);
    }

    [Fact]
    public async Task Handle_NoPersonasExist_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(userId));

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<Persona>());

        var command = new CreateFeatureCommand(product.Id, userId, "Feature", "Description");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Be("At least one persona is required before creating features");
    }

    [Fact]
    public async Task Handle_PersonaExists_CreatesFeature()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(userId));
        var persona = Persona.Create(product.Id, "User", "Developer");

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(new[] { persona });

        var command = new CreateFeatureCommand(product.Id, userId, "Feature", "Description");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.FeatureId.Should().NotBeEmpty();
    }
}

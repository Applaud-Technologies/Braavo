using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Products;
using FluentAssertions;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class CreateProductHandlerTests
{
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _handler = new CreateProductHandler(_productRepo);
    }

    [Fact]
    public async Task Handle_WithValidCommand_CreatesProduct()
    {
        var command = new CreateProductCommand(
            Name: "TaskFlow",
            Description: "A task management app",
            UserId: Guid.NewGuid()
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.ProductId.Should().NotBeEmpty();
        await _productRepo.Received(1).AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyName_ReturnsError()
    {
        var command = new CreateProductCommand(
            Name: "",
            Description: "Description",
            UserId: Guid.NewGuid()
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Be("Product name is required");
    }
}

// tests/Braavo.UnitTests/Entities/ProductTests.cs
using Braavo.Core.Entities;
using Braavo.Core.ValueObjects;
using FluentAssertions;

namespace Braavo.UnitTests.Core.Entities;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var userId = UserId.New();
        var name = "TaskFlow";
        var description = "A task management app";

        var product = Product.Create(name, description, userId);

        product.Id.Should().NotBeEmpty();
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.OwnerId.Should().Be(userId);
        product.Status.Should().Be(ProductStatus.Draft);
        product.Version.Should().Be(1);
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateOverview_UpdatesFieldsAndIncrementsVersion()
    {
        var product = Product.Create("Test", "Desc", UserId.New());
        var originalVersion = product.Version;

        product.UpdateOverview("New Vision", "New Problem", "New Value Prop");

        product.Vision.Should().Be("New Vision");
        product.ProblemStatement.Should().Be("New Problem");
        product.ValueProposition.Should().Be("New Value Prop");
        product.Version.Should().Be(originalVersion + 1);
    }

    [Fact]
    public void CreateVersion_CapturesCurrentState()
    {
        var product = Product.Create("Test", "Desc", UserId.New());
        product.UpdateOverview("Vision", "Problem", "Value");

        var version = product.CreateVersion("Initial draft");

        version.ProductId.Should().Be(product.Id);
        version.VersionNumber.Should().Be(product.Version);
        version.Snapshot.Should().Contain("Vision");
        version.Comment.Should().Be("Initial draft");
    }
}

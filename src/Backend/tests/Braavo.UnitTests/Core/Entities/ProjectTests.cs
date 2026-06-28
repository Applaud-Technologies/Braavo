using Braavo.Core.Entities;
using Braavo.Core.ValueObjects;
using FluentAssertions;

namespace Braavo.UnitTests.Core.Entities;

public class ProjectTests
{
    [Fact]
    public void Create_WithValidInput_ReturnsProject()
    {
        var ownerId = UserId.New();

        var project = Project.Create("My Project", ownerId, "A description");

        project.Id.Should().NotBeEmpty();
        project.Name.Should().Be("My Project");
        project.Description.Should().Be("A description");
        project.OwnerId.Should().Be(ownerId);
        project.Status.Should().Be("active");
        project.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithoutDescription_SetsNull()
    {
        var ownerId = UserId.New();

        var project = Project.Create("My Project", ownerId);

        project.Description.Should().BeNull();
    }

    [Fact]
    public void UpdateName_ChangesNameAndUpdatedAt()
    {
        var project = Project.Create("Original", UserId.New());
        var originalUpdatedAt = project.UpdatedAt;

        project.UpdateName("New Name");

        project.Name.Should().Be("New Name");
        project.UpdatedAt.Should().BeOnOrAfter(originalUpdatedAt);
    }
}

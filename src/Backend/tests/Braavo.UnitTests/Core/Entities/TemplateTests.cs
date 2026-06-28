// tests/Braavo.UnitTests/Core/Entities/TemplateTests.cs
using Braavo.Core.Entities;
using FluentAssertions;

namespace Braavo.UnitTests.Core.Entities;

public class TemplateTests
{
    [Fact]
    public void Create_WithValidInput_ReturnsTemplateWithCorrectProperties()
    {
        var template = Template.Create(
            "SaaS Application",
            "Template for B2B SaaS products",
            "Software",
            "# PRD Content",
            "Describe your SaaS product");

        template.Name.Should().Be("SaaS Application");
        template.Description.Should().Be("Template for B2B SaaS products");
        template.Category.Should().Be("Software");
        template.Content.Should().Be("# PRD Content");
        template.PromptHint.Should().Be("Describe your SaaS product");
    }

    [Fact]
    public void Create_AssignsNonEmptyGuid()
    {
        var template = Template.Create("Name", "Desc", "Cat", "Content", "Hint");

        template.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_SetsCreatedAtToUtcNow()
    {
        var before = DateTime.UtcNow;
        var template = Template.Create("Name", "Desc", "Cat", "Content", "Hint");
        var after = DateTime.UtcNow;

        template.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Create_DefaultsIsSystemToFalse()
    {
        var template = Template.Create("Name", "Desc", "Cat", "Content", "Hint");

        template.IsSystem.Should().BeFalse();
    }

    [Fact]
    public void Create_WithIsSystemTrue_SetsIsSystem()
    {
        var template = Template.Create("Name", "Desc", "Cat", "Content", "Hint", isSystem: true);

        template.IsSystem.Should().BeTrue();
    }
}

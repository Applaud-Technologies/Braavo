// tests/Braavo.UnitTests/Entities/FeatureTests.cs
using Braavo.Core.Entities;
using FluentAssertions;

namespace Braavo.UnitTests.Core.Entities;

public class FeatureTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var productId = Guid.NewGuid();
        var name = "Dashboard";
        var description = "Team task overview";

        var feature = Feature.Create(productId, name, description);

        feature.Id.Should().NotBeEmpty();
        feature.ProductId.Should().Be(productId);
        feature.Name.Should().Be(name);
        feature.Description.Should().Be(description);
        feature.Phase.Should().Be(FeaturePhase.Mvp);
        feature.LinkedStoryIds.Should().BeEmpty();
    }

    [Fact]
    public void LinkToStory_AddsStoryId()
    {
        var feature = Feature.Create(Guid.NewGuid(), "Feature", "Desc");
        var storyId = Guid.NewGuid();

        feature.LinkToStory(storyId);

        feature.LinkedStoryIds.Should().Contain(storyId);
    }

    [Fact]
    public void ChangePhase_UpdatesPhase()
    {
        var feature = Feature.Create(Guid.NewGuid(), "Feature", "Desc");

        feature.ChangePhase(FeaturePhase.Enhanced);

        feature.Phase.Should().Be(FeaturePhase.Enhanced);
    }
}

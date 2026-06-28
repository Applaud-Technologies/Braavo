using Braavo.Core.Entities;
using FluentAssertions;

namespace Braavo.UnitTests.Core.Entities;

public class UserStoryTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var productId = Guid.NewGuid();
        var asA = "team lead";
        var iWant = "to see all tasks in one dashboard";
        var soThat = "I can identify blockers quickly";

        var story = UserStory.Create(productId, asA, iWant, soThat);

        story.Id.Should().NotBeEmpty();
        story.ProductId.Should().Be(productId);
        story.AsA.Should().Be(asA);
        story.IWant.Should().Be(iWant);
        story.SoThat.Should().Be(soThat);
        story.Priority.Should().Be(StoryPriority.Should);
        story.AcceptanceCriteria.Should().BeEmpty();
    }

    [Fact]
    public void LinkToPersona_SetsPersonaId()
    {
        var story = UserStory.Create(Guid.NewGuid(), "user", "action", "benefit");
        var personaId = Guid.NewGuid();

        story.LinkToPersona(personaId);

        story.PersonaId.Should().Be(personaId);
    }

    [Fact]
    public void AddAcceptanceCriteria_AddsToList()
    {
        var story = UserStory.Create(Guid.NewGuid(), "user", "action", "benefit");

        story.AddAcceptanceCriteria("Given X, When Y, Then Z");

        story.AcceptanceCriteria.Should().Contain("Given X, When Y, Then Z");
    }
}

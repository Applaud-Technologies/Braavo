using Braavo.Core.Entities;
using FluentAssertions;

namespace Braavo.UnitTests.Entities;

public class PersonaTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var productId = Guid.NewGuid();
        var name = "Sarah";
        var role = "Team Lead";

        var persona = Persona.Create(productId, name, role);

        persona.Id.Should().NotBeEmpty();
        persona.ProductId.Should().Be(productId);
        persona.Name.Should().Be(name);
        persona.Role.Should().Be(role);
        persona.TechnicalLevel.Should().Be(TechnicalLevel.Medium);
        persona.Goals.Should().BeEmpty();
        persona.PainPoints.Should().BeEmpty();
    }

    [Fact]
    public void Update_UpdatesAllFields()
    {
        var persona = Persona.Create(Guid.NewGuid(), "Sarah", "Lead");
        var goals = new[] { "Track tasks", "Identify blockers" };
        var painPoints = new[] { "No visibility", "Manual updates" };

        persona.Update("Sarah Updated", "Senior Lead", TechnicalLevel.High, goals, painPoints, "I need better tools");

        persona.Name.Should().Be("Sarah Updated");
        persona.Role.Should().Be("Senior Lead");
        persona.TechnicalLevel.Should().Be(TechnicalLevel.High);
        persona.Goals.Should().BeEquivalentTo(goals);
        persona.PainPoints.Should().BeEquivalentTo(painPoints);
        persona.Quote.Should().Be("I need better tools");
    }
}

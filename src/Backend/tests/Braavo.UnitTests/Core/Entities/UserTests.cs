// tests/Braavo.UnitTests/Core/Entities/UserTests.cs
using Braavo.Core.Entities;
using Braavo.Core.ValueObjects;
using FluentAssertions;

namespace Braavo.UnitTests.Core.Entities;

public class UserTests
{
    [Fact]
    public void Create_WithValidInput_ReturnsUser()
    {
        var user = User.Create("test@example.com", "Test User");

        user.Id.Should().NotBe(UserId.Empty);
        user.Email.Value.Should().Be("test@example.com");
        user.Name.Should().Be("Test User");
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithInvalidEmail_Throws()
    {
        var act = () => User.Create("invalid", "Test User");

        act.Should().Throw<ArgumentException>();
    }
}

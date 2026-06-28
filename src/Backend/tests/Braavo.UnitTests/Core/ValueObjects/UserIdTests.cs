// tests/Braavo.UnitTests/Core/ValueObjects/UserIdTests.cs
using Braavo.Core.ValueObjects;
using FluentAssertions;

namespace Braavo.UnitTests.Core.ValueObjects;

public class UserIdTests
{
    [Fact]
    public void NewUserId_GeneratesUniqueId()
    {
        var id1 = UserId.New();
        var id2 = UserId.New();

        id1.Should().NotBe(id2);
    }

    [Fact]
    public void FromGuid_CreatesUserIdWithValue()
    {
        var guid = Guid.NewGuid();
        var userId = UserId.From(guid);

        userId.Value.Should().Be(guid);
    }
}

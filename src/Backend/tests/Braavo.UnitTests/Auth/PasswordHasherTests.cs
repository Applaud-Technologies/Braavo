// tests/Braavo.UnitTests/Auth/PasswordHasherTests.cs
using Braavo.Infrastructure.Auth;
using FluentAssertions;

namespace Braavo.UnitTests.Auth;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Hash_ReturnsNonEmptyString()
    {
        var hash = _hasher.Hash("password123");
        hash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Verify_WithCorrectPassword_ReturnsTrue()
    {
        var hash = _hasher.Hash("password123");
        _hasher.Verify("password123", hash).Should().BeTrue();
    }

    [Fact]
    public void Verify_WithWrongPassword_ReturnsFalse()
    {
        var hash = _hasher.Hash("password123");
        _hasher.Verify("wrongpassword", hash).Should().BeFalse();
    }
}

// tests/Braavo.UnitTests/Core/ValueObjects/EmailTests.cs
using Braavo.Core.ValueObjects;
using FluentAssertions;

namespace Braavo.UnitTests.Core.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("user@example.com", true)]
    [InlineData("invalid", false)]
    [InlineData("", false)]
    public void TryCreate_ValidatesEmailFormat(string input, bool shouldSucceed)
    {
        var result = Email.TryCreate(input, out var email);

        result.Should().Be(shouldSucceed);
        if (shouldSucceed)
            email!.Value.Value.Should().Be(input.ToLowerInvariant());
    }
}

using Braavo.Core.Models;
using Braavo.Infrastructure.ExternalServices;
using FluentAssertions;

namespace Braavo.UnitTests.ExternalServices;

public class MockLlmProviderTests
{
    [Fact]
    public async Task GenerateAsync_ReturnsResponse()
    {
        var provider = new MockLlmProvider();
        var request = new LlmRequest("Generate a PRD for a todo app");

        var response = await provider.GenerateAsync(request);

        response.Success.Should().BeTrue();
        response.Content.Should().NotBeNullOrEmpty();
    }
}

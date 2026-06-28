using Braavo.Core.Interfaces;
using Braavo.Core.Models;
using Braavo.Core.UseCases.Chat;
using FluentAssertions;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class SendChatMessageHandlerTests
{
    [Fact]
    public async Task Handle_WithValidMessage_ReturnsContent()
    {
        var llmProvider = Substitute.For<ILlmProvider>();
        llmProvider.GenerateAsync(Arg.Any<LlmRequest>(), Arg.Any<CancellationToken>())
            .Returns(new LlmResponse("Generated PRD content", 100, 200, true));

        var documentRepo = Substitute.For<IDocumentRepository>();

        var handler = new SendChatMessageHandler(llmProvider, documentRepo);
        var command = new SendChatMessageCommand(
            ProjectId: Guid.NewGuid(),
            DocumentId: null,
            Message: "Create a PRD for a todo app",
            UserId: Guid.NewGuid()
        );

        var result = await handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Content.Should().NotBeNullOrEmpty();
    }
}

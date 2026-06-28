using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.Models;
using Braavo.Core.UseCases.Wireframes;
using Braavo.Core.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class GenerateWireframeHandlerTests
{
    [Fact]
    public async Task Handle_GeneratesHtmlWireframe()
    {
        var userId = UserId.New();
        var document = Document.Create("Test PRD", DocumentType.Prd, Guid.NewGuid(), userId);
        document.UpdateContent("# PRD\n## Features\n- Login form\n- Dashboard");

        var documentRepo = Substitute.For<IDocumentRepository>();
        documentRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(document);

        var llmProvider = Substitute.For<ILlmProvider>();
        llmProvider.GenerateAsync(Arg.Any<LlmRequest>(), Arg.Any<CancellationToken>())
            .Returns(new LlmResponse("<div class='wireframe'><h1>Login</h1></div>", 100, 200, true));

        var handler = new GenerateWireframeHandler(llmProvider, documentRepo);
        var command = new GenerateWireframeCommand(Guid.NewGuid(), userId.Value, "Login Screen", "low");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Html.Should().Contain("wireframe");
    }
}

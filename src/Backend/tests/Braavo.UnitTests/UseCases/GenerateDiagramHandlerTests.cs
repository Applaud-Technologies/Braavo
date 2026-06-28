using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.Models;
using Braavo.Core.UseCases.Diagrams;
using Braavo.Core.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class GenerateDiagramHandlerTests
{
    [Fact]
    public async Task Handle_GeneratesFlowchart()
    {
        var ownerId = UserId.New();
        var document = Document.Create("Test PRD", DocumentType.Prd, Guid.NewGuid(), ownerId);
        document.UpdateContent("# PRD\n## Features\n- User login\n- Dashboard");

        var documentRepo = Substitute.For<IDocumentRepository>();
        documentRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(document);

        var llmProvider = Substitute.For<ILlmProvider>();
        llmProvider.GenerateAsync(Arg.Any<LlmRequest>(), Arg.Any<CancellationToken>())
            .Returns(new LlmResponse("flowchart TD\n    A[Start] --> B[Login]\n    B --> C[Dashboard]", 100, 200, true));

        var handler = new GenerateDiagramHandler(llmProvider, documentRepo);
        var command = new GenerateDiagramCommand(document.Id, DiagramType.Flowchart, ownerId.Value);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.MermaidCode.Should().Contain("flowchart");
    }
}

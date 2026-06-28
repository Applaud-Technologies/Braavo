using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.Models;
using Braavo.Core.UseCases.Prd;
using Braavo.Core.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class RefinePrdHandlerTests
{
    [Fact]
    public async Task Handle_WithNonOwner_ReturnsForbidden()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var ownerId = UserId.New();
        var attackerId = Guid.NewGuid(); // Different user

        var document = Document.Create("Test PRD", DocumentType.Prd, Guid.NewGuid(), ownerId);
        document.UpdateContent("# Title\n## Overview\nContent");

        var documentRepo = Substitute.For<IDocumentRepository>();
        documentRepo.GetByIdAsync(documentId, Arg.Any<CancellationToken>()).Returns(document);

        var llmProvider = Substitute.For<ILlmProvider>();
        var handler = new RefinePrdHandler(llmProvider, documentRepo);
        var command = new RefinePrdCommand(documentId, "Add more details", attackerId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Be("Forbidden");
    }

    [Fact]
    public async Task Handle_UpdatesDocumentContent()
    {
        var documentId = Guid.NewGuid();
        var ownerId = UserId.New();
        var document = Document.Create("Test PRD", DocumentType.Prd, Guid.NewGuid(), ownerId);
        document.UpdateContent("# Original PRD\n\n## Features\n- Feature 1");

        var documentRepo = Substitute.For<IDocumentRepository>();
        documentRepo.GetByIdAsync(documentId, Arg.Any<CancellationToken>()).Returns(document);

        var llmProvider = Substitute.For<ILlmProvider>();
        llmProvider.GenerateAsync(Arg.Any<LlmRequest>(), Arg.Any<CancellationToken>())
            .Returns(new LlmResponse("# Refined PRD\n\n## Features\n- Feature 1\n- Feature 2", 100, 200, true));

        var handler = new RefinePrdHandler(llmProvider, documentRepo);
        var command = new RefinePrdCommand(documentId, "Add another feature", ownerId.Value);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Content.Should().Contain("Feature 2");
        await documentRepo.Received(1).UpdateAsync(Arg.Any<Document>(), Arg.Any<CancellationToken>());
    }
}

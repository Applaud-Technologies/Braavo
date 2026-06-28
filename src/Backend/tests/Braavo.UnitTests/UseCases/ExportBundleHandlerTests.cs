using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Export;
using Braavo.Core.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class ExportBundleHandlerTests
{
    [Fact]
    public async Task Handle_GeneratesZipBundle()
    {
        var document = Document.Create("Test PRD", DocumentType.Prd, Guid.NewGuid(), UserId.New());
        document.UpdateContent("""
            # Test Product PRD

            ## Overview
            A test product.

            ## User Stories
            - As a user, I want to login so that I can access my account
            """);

        var documentRepo = Substitute.For<IDocumentRepository>();
        documentRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(document);

        var handler = new ExportBundleHandler(documentRepo);
        var command = new ExportBundleCommand(Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.ZipContent.Should().NotBeEmpty();
        result.FileName.Should().EndWith(".zip");
    }
}

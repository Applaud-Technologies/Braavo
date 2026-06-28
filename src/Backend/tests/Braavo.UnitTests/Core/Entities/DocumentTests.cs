// tests/Braavo.UnitTests/Core/Entities/DocumentTests.cs
using Braavo.Core.Entities;
using Braavo.Core.ValueObjects;
using FluentAssertions;

namespace Braavo.UnitTests.Core.Entities;

public class DocumentTests
{
    [Fact]
    public void Create_WithValidInput_ReturnsDocument()
    {
        var projectId = Guid.NewGuid();
        var userId = UserId.New();

        var doc = Document.Create("My PRD", DocumentType.Prd, projectId, userId);

        doc.Id.Should().NotBeEmpty();
        doc.Title.Should().Be("My PRD");
        doc.Type.Should().Be(DocumentType.Prd);
        doc.ProjectId.Should().Be(projectId);
        doc.CreatedBy.Should().Be(userId);
        doc.Version.Should().Be(1);
    }

    [Fact]
    public void UpdateContent_IncrementsVersion()
    {
        var doc = Document.Create("Test", DocumentType.Prd, Guid.NewGuid(), UserId.New());
        var originalVersion = doc.Version;

        doc.UpdateContent("{\"sections\": []}");

        doc.Version.Should().Be(originalVersion + 1);
        doc.Content.Should().Be("{\"sections\": []}");
    }
}

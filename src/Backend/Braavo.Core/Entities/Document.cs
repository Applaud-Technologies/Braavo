using Braavo.Core.ValueObjects;

namespace Braavo.Core.Entities;

public enum DocumentType
{
    Prd,
    Wireframe,
    Diagram,
    Code
}

public class Document
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = "{}";
    public DocumentType Type { get; private set; }
    public int Version { get; private set; }
    public UserId CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Document() { }

    public static Document Create(string title, DocumentType type, Guid projectId, UserId createdBy)
    {
        return new Document
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Title = title,
            Type = type,
            Version = 1,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateContent(string content)
    {
        Content = content;
        Version++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTitle(string title)
    {
        Title = title;
        UpdatedAt = DateTime.UtcNow;
    }
}

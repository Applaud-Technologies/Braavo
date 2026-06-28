// src/Backend/Braavo.Core/Entities/Template.cs
namespace Braavo.Core.Entities;

public class Template
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public string PromptHint { get; private set; } = string.Empty;
    public bool IsSystem { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Template() { }

    public static Template Create(
        string name,
        string description,
        string category,
        string content,
        string promptHint,
        bool isSystem = false)
    {
        return new Template
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Category = category,
            Content = content,
            PromptHint = promptHint,
            IsSystem = isSystem,
            CreatedAt = DateTime.UtcNow
        };
    }
}

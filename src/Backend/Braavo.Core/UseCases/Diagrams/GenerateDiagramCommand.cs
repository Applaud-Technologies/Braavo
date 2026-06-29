using MediatR;

namespace Braavo.Core.UseCases.Diagrams;

public enum DiagramType
{
    Flowchart,
    Sequence,
    ClassDiagram,
    EntityRelationship,
    UserJourney,
    Component
}

public record GenerateDiagramCommand(
    Guid DocumentId,
    DiagramType Type,
    Guid UserId,
    string? Focus = null
) : IRequest<DiagramResponse>;

public record DiagramResponse(
    string MermaidCode,
    DiagramType Type,
    bool Success,
    string? Error = null
);

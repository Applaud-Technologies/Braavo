using MediatR;

namespace Braavo.Core.UseCases.Diagrams;

public enum DiagramType
{
    Flowchart,
    Sequence,
    ClassDiagram,
    EntityRelationship,
    UserJourney
}

public record GenerateDiagramCommand(
    Guid DocumentId,
    DiagramType Type,
    string? Focus
) : IRequest<DiagramResponse>;

public record DiagramResponse(
    string MermaidCode,
    DiagramType Type,
    bool Success,
    string? Error = null
);

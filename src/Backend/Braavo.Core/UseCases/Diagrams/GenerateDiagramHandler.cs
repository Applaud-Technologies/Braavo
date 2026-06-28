using Braavo.Core.Interfaces;
using Braavo.Core.Models;
using MediatR;

namespace Braavo.Core.UseCases.Diagrams;

public class GenerateDiagramHandler : IRequestHandler<GenerateDiagramCommand, DiagramResponse>
{
    private readonly ILlmProvider _llm;
    private readonly IDocumentRepository _documents;

    public GenerateDiagramHandler(ILlmProvider llm, IDocumentRepository documents)
    {
        _llm = llm;
        _documents = documents;
    }

    public async Task<DiagramResponse> Handle(GenerateDiagramCommand request, CancellationToken ct)
    {
        var document = await _documents.GetByIdAsync(request.DocumentId, ct);
        if (document is null)
            return new DiagramResponse("", request.Type, false, "Document not found");

        var systemPrompt = GetSystemPrompt(request.Type);
        var userPrompt = BuildUserPrompt(document.Content, request.Type, request.Focus);

        var llmRequest = new LlmRequest(userPrompt, systemPrompt, 2048, 0.3);
        var response = await _llm.GenerateAsync(llmRequest, ct);

        if (!response.Success)
            return new DiagramResponse("", request.Type, false, response.Error);

        var mermaidCode = ExtractMermaidCode(response.Content);
        return new DiagramResponse(mermaidCode, request.Type, true);
    }

    private static string GetSystemPrompt(DiagramType type) => type switch
    {
        DiagramType.Flowchart => """
            Generate a Mermaid.js flowchart diagram based on the PRD content.
            Use 'flowchart TD' for top-down layout.
            Include main user flows and decision points.
            Return ONLY the Mermaid code, no explanation.
            """,
        DiagramType.Sequence => """
            Generate a Mermaid.js sequence diagram based on the PRD content.
            Show interactions between users and system components.
            Return ONLY the Mermaid code, no explanation.
            """,
        DiagramType.ClassDiagram => """
            Generate a Mermaid.js class diagram based on the PRD content.
            Identify main entities and their relationships.
            Return ONLY the Mermaid code, no explanation.
            """,
        DiagramType.EntityRelationship => """
            Generate a Mermaid.js ER diagram based on the PRD content.
            Show database entities and their relationships.
            Return ONLY the Mermaid code, no explanation.
            """,
        DiagramType.UserJourney => """
            Generate a Mermaid.js user journey diagram based on the PRD content.
            Map out the main user experience flow.
            Return ONLY the Mermaid code, no explanation.
            """,
        _ => "Generate a Mermaid.js diagram. Return ONLY the Mermaid code."
    };

    private static string BuildUserPrompt(string content, DiagramType type, string? focus)
    {
        var focusText = string.IsNullOrEmpty(focus) ? "" : $"\nFocus on: {focus}";
        return $"""
            PRD Content:
            {content}
            {focusText}

            Generate a {type} diagram for this PRD.
            """;
    }

    private static string ExtractMermaidCode(string content)
    {
        if (content.Contains("```mermaid"))
        {
            var start = content.IndexOf("```mermaid") + 10;
            var end = content.IndexOf("```", start);
            if (end > start)
                return content[start..end].Trim();
        }

        if (content.Contains("```"))
        {
            var start = content.IndexOf("```") + 3;
            var end = content.IndexOf("```", start);
            if (end > start)
                return content[start..end].Trim();
        }

        return content.Trim();
    }
}

using Braavo.Core.Interfaces;
using Braavo.Core.Models;
using MediatR;

namespace Braavo.Core.UseCases.Wireframes;

public class GenerateWireframeHandler : IRequestHandler<GenerateWireframeCommand, WireframeResponse>
{
    private readonly ILlmProvider _llm;
    private readonly IDocumentRepository _documents;

    public GenerateWireframeHandler(ILlmProvider llm, IDocumentRepository documents)
    {
        _llm = llm;
        _documents = documents;
    }

    public async Task<WireframeResponse> Handle(GenerateWireframeCommand request, CancellationToken ct)
    {
        var document = await _documents.GetByIdAsync(request.DocumentId, ct);
        if (document is null)
            return new WireframeResponse("", "", false, "Document not found");

        var systemPrompt = GetSystemPrompt(request.Fidelity);
        var userPrompt = BuildUserPrompt(document.Content, request.ScreenName);

        var llmRequest = new LlmRequest(userPrompt, systemPrompt, 4096, 0.5);
        var response = await _llm.GenerateAsync(llmRequest, ct);

        if (!response.Success)
            return new WireframeResponse("", "", false, response.Error);

        var html = ExtractHtml(response.Content);
        var screenName = request.ScreenName ?? "Main Screen";

        return new WireframeResponse(html, screenName, true);
    }

    private static string GetSystemPrompt(string fidelity) => fidelity.ToLower() switch
    {
        "low" => """
            Generate a low-fidelity HTML wireframe based on the PRD.
            Use simple gray boxes, placeholder text, and basic layout.
            Style with inline CSS using these colors:
            - Background: #f5f5f5
            - Boxes: #e0e0e0 with 1px solid #ccc border
            - Text placeholders: #999
            - Buttons: #ddd with rounded corners
            Return ONLY the HTML code wrapped in a single div with class 'wireframe'.
            Use flexbox for layout. Make it responsive.
            """,
        "high" => """
            Generate a high-fidelity HTML wireframe based on the PRD.
            Use realistic content, proper typography, and polished styling.
            Style with inline CSS using a blue primary color (#3b82f6).
            Include realistic placeholder images and text.
            Return ONLY the HTML code wrapped in a single div with class 'wireframe'.
            """,
        _ => """
            Generate a medium-fidelity HTML wireframe based on the PRD.
            Use basic styling with some color, placeholder content.
            Return ONLY the HTML code wrapped in a single div with class 'wireframe'.
            """
    };

    private static string BuildUserPrompt(string content, string? screenName)
    {
        var screen = string.IsNullOrEmpty(screenName)
            ? "the main screen/dashboard"
            : screenName;

        return $"""
            PRD Content:
            {content}

            Generate a wireframe for: {screen}

            Include appropriate UI elements based on the features described.
            """;
    }

    private static string ExtractHtml(string content)
    {
        if (content.Contains("```html"))
        {
            var start = content.IndexOf("```html") + 7;
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

        if (content.Contains("<div"))
        {
            var start = content.IndexOf("<div");
            var end = content.LastIndexOf("</div>") + 6;
            if (end > start)
                return content[start..end];
        }

        return $"<div class='wireframe'>{content}</div>";
    }
}

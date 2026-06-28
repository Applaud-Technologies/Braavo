using Braavo.Core.Interfaces;
using Braavo.Core.Models;
using MediatR;

namespace Braavo.Core.UseCases.AI;

public record RefineContentCommand(
    Guid UserId,
    string Content,
    string ContentType,  // "goal", "painPoint", "story", "description", "acceptanceCriteria"
    string? Instruction  // Optional: "make more specific", "add metrics", "simplify"
) : IRequest<RefineContentResult>;

public record RefineContentResult(
    bool Success,
    string? RefinedContent = null,
    string? Error = null
);

public class RefineContentHandler : IRequestHandler<RefineContentCommand, RefineContentResult>
{
    private readonly ILlmProvider _llmProvider;

    private static readonly Dictionary<string, string> SystemPromptsByContentType = new(StringComparer.OrdinalIgnoreCase)
    {
        ["goal"] = "Improve this user goal to be more specific and actionable",
        ["painPoint"] = "Improve this pain point to be more specific and impactful",
        ["story"] = "Improve this user story to follow best practices (clear, testable, valuable)",
        ["description"] = "Improve this description to be clearer and more professional",
        ["acceptanceCriteria"] = "Improve this acceptance criterion to be specific and testable",
    };

    public RefineContentHandler(ILlmProvider llmProvider)
    {
        _llmProvider = llmProvider;
    }

    public async Task<RefineContentResult> Handle(RefineContentCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return new RefineContentResult(false, Error: "Content is required");

        if (!SystemPromptsByContentType.TryGetValue(request.ContentType, out var baseSystemPrompt))
            return new RefineContentResult(false, Error: $"Unknown content type: {request.ContentType}");

        var systemPrompt = string.IsNullOrWhiteSpace(request.Instruction)
            ? baseSystemPrompt
            : $"{baseSystemPrompt}\nAdditional instruction: {request.Instruction}";

        var llmRequest = new LlmRequest(
            Prompt: request.Content,
            SystemPrompt: systemPrompt + "\nReturn only the improved text, no explanation or preamble.",
            MaxTokens: 512,
            Temperature: 0.5
        );

        var llmResponse = await _llmProvider.GenerateAsync(llmRequest, ct);

        if (!llmResponse.Success)
            return new RefineContentResult(false, Error: llmResponse.Error ?? "LLM generation failed");

        return new RefineContentResult(true, RefinedContent: llmResponse.Content?.Trim());
    }
}

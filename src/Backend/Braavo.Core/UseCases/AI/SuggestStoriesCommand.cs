using System.Text.Json;
using Braavo.Core.Interfaces;
using Braavo.Core.Models;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.AI;

public record SuggestStoriesCommand(
    Guid ProductId,
    Guid UserId,
    Guid PersonaId,
    string? AdditionalContext = null
) : IRequest<SuggestStoriesResult>;

public record SuggestedStory(
    string AsA,
    string IWant,
    string SoThat,
    string Priority,  // "Must", "Should", "Could", "Wont"
    string[] AcceptanceCriteria
);

public record SuggestStoriesResult(
    bool Success,
    SuggestedStory[]? Stories = null,
    string? Error = null
);

public class SuggestStoriesHandler : IRequestHandler<SuggestStoriesCommand, SuggestStoriesResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IPersonaRepository _personaRepo;
    private readonly ILlmProvider _llmProvider;

    private const string SystemPrompt = """
        You are a product manager assistant. Based on the persona details, suggest 3-5 user stories.
        Return ONLY valid JSON array:
        [
          {
            "asA": "the user role",
            "iWant": "what they want to do",
            "soThat": "the benefit they get",
            "priority": "Must" or "Should" or "Could",
            "acceptanceCriteria": ["criterion 1", "criterion 2"]
          }
        ]
        """;

    public SuggestStoriesHandler(
        IProductRepository productRepo,
        IPersonaRepository personaRepo,
        ILlmProvider llmProvider)
    {
        _productRepo = productRepo;
        _personaRepo = personaRepo;
        _llmProvider = llmProvider;
    }

    public async Task<SuggestStoriesResult> Handle(SuggestStoriesCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new SuggestStoriesResult(false, Error: "Product not found");

        var persona = await _personaRepo.GetByIdAsync(request.PersonaId, ct);
        if (persona is null || persona.ProductId != request.ProductId)
            return new SuggestStoriesResult(false, Error: "Persona not found");

        var promptBuilder = new System.Text.StringBuilder();
        promptBuilder.AppendLine($"Persona: {persona.Name}");
        promptBuilder.AppendLine($"Role: {persona.Role}");
        promptBuilder.AppendLine($"Technical Level: {persona.TechnicalLevel}");

        if (persona.Goals.Length > 0)
            promptBuilder.AppendLine($"Goals: {string.Join(", ", persona.Goals)}");

        if (persona.PainPoints.Length > 0)
            promptBuilder.AppendLine($"Pain Points: {string.Join(", ", persona.PainPoints)}");

        if (!string.IsNullOrWhiteSpace(persona.Quote))
            promptBuilder.AppendLine($"Quote: \"{persona.Quote}\"");

        if (!string.IsNullOrWhiteSpace(request.AdditionalContext))
            promptBuilder.AppendLine($"\nAdditional Context: {request.AdditionalContext}");

        var llmRequest = new LlmRequest(
            Prompt: promptBuilder.ToString(),
            SystemPrompt: SystemPrompt,
            MaxTokens: 2048,
            Temperature: 0.7
        );

        var llmResponse = await _llmProvider.GenerateAsync(llmRequest, ct);

        if (!llmResponse.Success)
            return new SuggestStoriesResult(false, Error: llmResponse.Error ?? "LLM generation failed");

        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var stories = JsonSerializer.Deserialize<SuggestedStory[]>(llmResponse.Content, options);

            if (stories is null || stories.Length == 0)
                return new SuggestStoriesResult(false, Error: "Failed to parse stories from LLM response");

            return new SuggestStoriesResult(true, Stories: stories);
        }
        catch (JsonException ex)
        {
            return new SuggestStoriesResult(false, Error: $"Failed to parse LLM response as JSON: {ex.Message}");
        }
    }
}

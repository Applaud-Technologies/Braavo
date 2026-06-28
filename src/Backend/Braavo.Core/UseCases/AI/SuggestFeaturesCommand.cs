using System.Text.Json;
using Braavo.Core.Interfaces;
using Braavo.Core.Models;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.AI;

public record SuggestFeaturesCommand(
    Guid ProductId,
    Guid UserId,
    Guid[]? StoryIds = null  // If null, use all stories for the product
) : IRequest<SuggestFeaturesResult>;

public record SuggestedFeature(
    string Name,
    string Description,
    string Phase,  // "Mvp", "Enhanced", "Future"
    string? Effort,  // "Small", "Medium", "Large", or null
    Guid[] LinkedStoryIds  // Which stories this feature addresses
);

public record SuggestFeaturesResult(
    bool Success,
    SuggestedFeature[]? Features = null,
    string? Error = null
);

// Internal DTO for deserializing LLM response before mapping story indices to Guids
file record LlmFeature(
    string Name,
    string Description,
    string Phase,
    string? Effort,
    int[] AddressesStories
);

public class SuggestFeaturesHandler : IRequestHandler<SuggestFeaturesCommand, SuggestFeaturesResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IUserStoryRepository _storyRepo;
    private readonly ILlmProvider _llmProvider;

    private const string SystemPrompt = """
        You are a product manager assistant. Based on the user stories provided, suggest 3-5 features.
        For each feature, indicate which story numbers (1-based index) it addresses.
        Return ONLY valid JSON array:
        [
          {
            "name": "Feature Name",
            "description": "What this feature does",
            "phase": "Mvp" or "Enhanced" or "Future",
            "effort": "Small" or "Medium" or "Large",
            "addressesStories": [1, 2]
          }
        ]
        """;

    public SuggestFeaturesHandler(
        IProductRepository productRepo,
        IUserStoryRepository storyRepo,
        ILlmProvider llmProvider)
    {
        _productRepo = productRepo;
        _storyRepo = storyRepo;
        _llmProvider = llmProvider;
    }

    public async Task<SuggestFeaturesResult> Handle(SuggestFeaturesCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new SuggestFeaturesResult(false, Error: "Product not found");

        // Fetch stories — either the specified ones or all for the product
        var stories = request.StoryIds is { Length: > 0 }
            ? (await Task.WhenAll(request.StoryIds.Select(id => _storyRepo.GetByIdAsync(id, ct))))
                .Where(s => s is not null && s.ProductId == request.ProductId)
                .Select(s => s!)
                .ToArray()
            : (await _storyRepo.GetByProductIdAsync(request.ProductId, ct)).ToArray();

        if (stories.Length == 0)
            return new SuggestFeaturesResult(false, Error: "No stories found for this product");

        var promptBuilder = new System.Text.StringBuilder();
        promptBuilder.AppendLine("User Stories:");
        for (var i = 0; i < stories.Length; i++)
        {
            var s = stories[i];
            promptBuilder.AppendLine($"{i + 1}. As a {s.AsA}, I want {s.IWant}, so that {s.SoThat}");
        }

        var llmRequest = new LlmRequest(
            Prompt: promptBuilder.ToString(),
            SystemPrompt: SystemPrompt,
            MaxTokens: 2048,
            Temperature: 0.7
        );

        var llmResponse = await _llmProvider.GenerateAsync(llmRequest, ct);

        if (!llmResponse.Success)
            return new SuggestFeaturesResult(false, Error: llmResponse.Error ?? "LLM generation failed");

        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var llmFeatures = JsonSerializer.Deserialize<LlmFeature[]>(llmResponse.Content, options);

            if (llmFeatures is null || llmFeatures.Length == 0)
                return new SuggestFeaturesResult(false, Error: "Failed to parse features from LLM response");

            // Map 1-based story indices back to actual Guids
            var features = llmFeatures.Select(f => new SuggestedFeature(
                Name: f.Name,
                Description: f.Description,
                Phase: f.Phase,
                Effort: f.Effort,
                LinkedStoryIds: f.AddressesStories
                    .Where(idx => idx >= 1 && idx <= stories.Length)
                    .Select(idx => stories[idx - 1].Id)
                    .ToArray()
            )).ToArray();

            return new SuggestFeaturesResult(true, Features: features);
        }
        catch (JsonException ex)
        {
            return new SuggestFeaturesResult(false, Error: $"Failed to parse LLM response as JSON: {ex.Message}");
        }
    }
}

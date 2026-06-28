using System.Text.Json;
using Braavo.Core.Interfaces;
using Braavo.Core.Models;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.AI;

public record GeneratePersonaCommand(
    Guid ProductId,
    Guid UserId,
    string Description
) : IRequest<GeneratePersonaResult>;

public record GeneratedPersona(
    string Name,
    string Role,
    string TechnicalLevel,
    string[] Goals,
    string[] PainPoints,
    string Quote
);

public record GeneratePersonaResult(
    bool Success,
    GeneratedPersona? Persona = null,
    string? Error = null
);

public class GeneratePersonaHandler : IRequestHandler<GeneratePersonaCommand, GeneratePersonaResult>
{
    private readonly IProductRepository _productRepo;
    private readonly ILlmProvider _llmProvider;

    private const string SystemPrompt = """
        You are a product manager assistant. Generate a detailed user persona based on the description provided.
        Return ONLY valid JSON with these exact fields:
        {
          "name": "Persona Name",
          "role": "Their job title or role",
          "technicalLevel": "Low" or "Medium" or "High",
          "goals": ["goal 1", "goal 2", "goal 3"],
          "painPoints": ["pain point 1", "pain point 2", "pain point 3"],
          "quote": "A representative quote from this persona"
        }
        """;

    public GeneratePersonaHandler(IProductRepository productRepo, ILlmProvider llmProvider)
    {
        _productRepo = productRepo;
        _llmProvider = llmProvider;
    }

    public async Task<GeneratePersonaResult> Handle(GeneratePersonaCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new GeneratePersonaResult(false, Error: "Product not found");

        if (string.IsNullOrWhiteSpace(request.Description))
            return new GeneratePersonaResult(false, Error: "Description is required");

        var llmRequest = new LlmRequest(
            Prompt: request.Description,
            SystemPrompt: SystemPrompt,
            MaxTokens: 1024,
            Temperature: 0.7
        );

        var llmResponse = await _llmProvider.GenerateAsync(llmRequest, ct);

        if (!llmResponse.Success)
            return new GeneratePersonaResult(false, Error: llmResponse.Error ?? "LLM generation failed");

        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var persona = JsonSerializer.Deserialize<GeneratedPersona>(llmResponse.Content, options);

            if (persona is null)
                return new GeneratePersonaResult(false, Error: "Failed to parse persona from LLM response");

            return new GeneratePersonaResult(true, Persona: persona);
        }
        catch (JsonException ex)
        {
            return new GeneratePersonaResult(false, Error: $"Failed to parse LLM response as JSON: {ex.Message}");
        }
    }
}

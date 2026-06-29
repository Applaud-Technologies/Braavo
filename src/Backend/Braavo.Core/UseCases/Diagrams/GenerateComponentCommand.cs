using System.Text;
using Braavo.Core.Interfaces;
using Braavo.Core.Models;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Diagrams;

public record GenerateComponentCommand(Guid ProductId, Guid UserId) : IRequest<GenerateComponentResult>;

public record GenerateComponentResult(
    bool Success,
    string? MermaidCode = null,
    string? Error = null,
    bool IsUnprocessableEntity = false
);

public class GenerateComponentHandler : IRequestHandler<GenerateComponentCommand, GenerateComponentResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IFeatureRepository _featureRepo;
    private readonly ILlmProvider _llm;

    public GenerateComponentHandler(
        IProductRepository productRepo,
        IFeatureRepository featureRepo,
        ILlmProvider llm)
    {
        _productRepo = productRepo;
        _featureRepo = featureRepo;
        _llm = llm;
    }

    public async Task<GenerateComponentResult> Handle(GenerateComponentCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new GenerateComponentResult(false, Error: "Product not found");

        var features = await _featureRepo.GetByProductIdAsync(request.ProductId, ct);
        if (features.Count == 0)
            return new GenerateComponentResult(false, Error: "No features found", IsUnprocessableEntity: true);

        var prdContent = BuildPrdContent(features);

        var systemPrompt = """
            Generate a Mermaid.js flowchart diagram representing a UML-style component architecture.
            Use 'flowchart TB' for top-to-bottom layout.
            Identify system components (frontend, backend, services, databases, external APIs).
            Show dependencies between components with labeled arrows.
            Group related components in subgraphs.
            Use descriptive node names like 'WebApp[Web Application]', 'API[REST API]', 'DB[(Database)]'.
            Return ONLY the Mermaid code, no explanation.
            """;

        var userPrompt = $"""
            Product: {product.Name}

            Vision: {product.Vision}

            Problem Statement: {product.ProblemStatement}

            Features:
            {prdContent}

            Generate a component architecture diagram for this product.
            """;

        var llmRequest = new LlmRequest(userPrompt, systemPrompt, 2048, 0.3);
        var response = await _llm.GenerateAsync(llmRequest, ct);

        if (!response.Success)
            return new GenerateComponentResult(false, Error: response.Error);

        var mermaidCode = ExtractMermaidCode(response.Content);
        return new GenerateComponentResult(true, MermaidCode: mermaidCode);
    }

    private static string BuildPrdContent(IReadOnlyList<Entities.Feature> features)
    {
        var sb = new StringBuilder();
        foreach (var feature in features.OrderBy(f => f.Phase).ThenBy(f => f.SortOrder))
        {
            sb.AppendLine($"- {feature.Name}: {feature.Description}");
        }
        return sb.ToString();
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

using System.Text;
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Diagrams;

public record GenerateFeatureHierarchyCommand(Guid ProductId, Guid UserId) : IRequest<GenerateFeatureHierarchyResult>;

public record GenerateFeatureHierarchyResult(
    bool Success,
    string? MermaidCode = null,
    string? Error = null,
    bool IsUnprocessableEntity = false
);

public class GenerateFeatureHierarchyHandler : IRequestHandler<GenerateFeatureHierarchyCommand, GenerateFeatureHierarchyResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IFeatureRepository _featureRepo;

    public GenerateFeatureHierarchyHandler(IProductRepository productRepo, IFeatureRepository featureRepo)
    {
        _productRepo = productRepo;
        _featureRepo = featureRepo;
    }

    public async Task<GenerateFeatureHierarchyResult> Handle(GenerateFeatureHierarchyCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new GenerateFeatureHierarchyResult(false, Error: "Product not found");

        var features = await _featureRepo.GetByProductIdAsync(request.ProductId, ct);

        if (features.Count == 0)
            return new GenerateFeatureHierarchyResult(false, Error: "No features found", IsUnprocessableEntity: true);

        var byPhase = features
            .GroupBy(f => f.Phase)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.OrderBy(f => f.SortOrder).ThenBy(f => f.Name).ToList());

        var mermaidCode = GenerateMermaidMindmap(product.Name, byPhase);

        return new GenerateFeatureHierarchyResult(true, MermaidCode: mermaidCode);
    }

    private static string GenerateMermaidMindmap(
        string productName,
        Dictionary<FeaturePhase, List<Feature>> byPhase)
    {
        var sb = new StringBuilder();
        sb.AppendLine("mindmap");
        sb.AppendLine($"  root(({EscapeLabel(productName)}))");

        foreach (var (phase, features) in byPhase)
        {
            var phaseLabel = PhaseDisplayName(phase);
            sb.AppendLine($"    {phaseLabel}");

            foreach (var feature in features)
            {
                sb.AppendLine($"      {EscapeLabel(feature.Name)}");
            }
        }

        return sb.ToString().TrimEnd();
    }

    private static string PhaseDisplayName(FeaturePhase phase) => phase switch
    {
        FeaturePhase.Mvp      => "MVP",
        FeaturePhase.Enhanced => "Enhanced",
        FeaturePhase.Future   => "Future",
        _                     => phase.ToString()
    };

    /// <summary>Strips parentheses and newlines that would break Mermaid mindmap node labels.</summary>
    private static string EscapeLabel(string text)
        => text.Replace("(", "").Replace(")", "").Replace("\n", " ").Replace("\r", "");
}

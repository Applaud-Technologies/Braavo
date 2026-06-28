using Braavo.Core.Interfaces;

namespace Braavo.Core.Services;

/// <summary>
/// Domain service that calculates a product's completion percentage
/// based on the presence of key PRD sections.
/// </summary>
public static class CompletionCalculator
{
    // Weights per spec (must sum to 100)
    private const int VisionWeight           = 10;
    private const int ProblemStatementWeight = 10;
    private const int PersonaWeight          = 15;
    private const int UserStoryWeight        = 15;
    private const int FeatureWeight          = 15;
    private const int TimelineWeight         = 15;
    private const int ValuePropositionWeight = 10;
    private const int TargetMarketWeight     = 10;

    public static async Task<int> CalculateAsync(
        Guid productId,
        string vision,
        string problemStatement,
        string valueProposition,
        string[] targetMarket,
        IPersonaRepository personaRepo,
        IUserStoryRepository userStoryRepo,
        IFeatureRepository featureRepo,
        ITimelineRepository timelineRepo,
        CancellationToken ct = default)
    {
        var percentage = 0;

        if (!string.IsNullOrWhiteSpace(vision))
            percentage += VisionWeight;

        if (!string.IsNullOrWhiteSpace(problemStatement))
            percentage += ProblemStatementWeight;

        if (!string.IsNullOrWhiteSpace(valueProposition))
            percentage += ValuePropositionWeight;

        if (targetMarket is { Length: > 0 })
            percentage += TargetMarketWeight;

        var personas = await personaRepo.GetByProductIdAsync(productId, ct);
        if (personas.Count >= 1)
            percentage += PersonaWeight;

        var userStories = await userStoryRepo.GetByProductIdAsync(productId, ct);
        if (userStories.Count >= 3)
            percentage += UserStoryWeight;

        var features = await featureRepo.GetByProductIdAsync(productId, ct);
        if (features.Count >= 3)
            percentage += FeatureWeight;

        var timelinePhases = await timelineRepo.GetByProductIdAsync(productId, ct);
        if (timelinePhases.Count >= 1)
            percentage += TimelineWeight;

        return percentage;
    }
}

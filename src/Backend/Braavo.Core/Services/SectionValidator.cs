using Braavo.Core.Entities;
using Braavo.Core.Interfaces;

namespace Braavo.Core.Services;

/// <summary>
/// Domain service that validates each PRD section for completeness
/// and returns per-section warnings.
/// </summary>
public static class SectionValidator
{
    public record SectionValidation(
        string Section,
        bool IsValid,
        string[] Warnings);

    public static async Task<IReadOnlyList<SectionValidation>> ValidateAsync(
        Guid productId,
        string vision,
        string problemStatement,
        IPersonaRepository personaRepo,
        IUserStoryRepository userStoryRepo,
        IFeatureRepository featureRepo,
        ITimelineRepository timelineRepo,
        CancellationToken ct = default)
    {
        var results = new List<SectionValidation>
        {
            ValidateOverview(vision, problemStatement)
        };

        var personas = await personaRepo.GetByProductIdAsync(productId, ct);
        results.Add(ValidatePersonas(personas));

        var stories = await userStoryRepo.GetByProductIdAsync(productId, ct);
        results.Add(ValidateUserStories(stories));

        var features = await featureRepo.GetByProductIdAsync(productId, ct);
        results.Add(ValidateFeatures(features));

        var timelinePhases = await timelineRepo.GetByProductIdAsync(productId, ct);
        results.Add(ValidateTimeline(timelinePhases));

        return results.AsReadOnly();
    }

    // ── private section validators ────────────────────────────────────────────

    private static SectionValidation ValidateOverview(string vision, string problemStatement)
    {
        var warnings = new List<string>();

        if (string.IsNullOrWhiteSpace(vision))
            warnings.Add("Vision statement is required.");

        if (string.IsNullOrWhiteSpace(problemStatement))
            warnings.Add("Problem statement is required.");

        return new SectionValidation("Overview", warnings.Count == 0, [.. warnings]);
    }

    private static SectionValidation ValidatePersonas(IReadOnlyList<Persona> personas)
    {
        var warnings = new List<string>();

        if (personas.Count == 0)
        {
            warnings.Add("At least one persona is required.");
        }
        else
        {
            foreach (var p in personas)
            {
                if (string.IsNullOrWhiteSpace(p.Name))
                    warnings.Add($"Persona is missing a name.");
                if (string.IsNullOrWhiteSpace(p.Role))
                    warnings.Add($"Persona '{p.Name}' is missing a role.");
            }
        }

        return new SectionValidation("Personas", warnings.Count == 0, [.. warnings]);
    }

    private static SectionValidation ValidateUserStories(IReadOnlyList<UserStory> stories)
    {
        var warnings = new List<string>();

        if (stories.Count == 0)
        {
            warnings.Add("At least one user story is required.");
        }
        else
        {
            var completeCount = stories.Count(s =>
                !string.IsNullOrWhiteSpace(s.AsA) &&
                !string.IsNullOrWhiteSpace(s.IWant) &&
                !string.IsNullOrWhiteSpace(s.SoThat));

            if (completeCount == 0)
                warnings.Add("At least one user story must have all three parts: As a, I want, So that.");
        }

        return new SectionValidation("UserStories", warnings.Count == 0, [.. warnings]);
    }

    private static SectionValidation ValidateFeatures(IReadOnlyList<Feature> features)
    {
        var warnings = new List<string>();

        var mvpFeatures = features.Where(f => f.Phase == FeaturePhase.Mvp).ToList();
        if (mvpFeatures.Count == 0)
            warnings.Add("At least one MVP feature is required.");

        return new SectionValidation("Features", warnings.Count == 0, [.. warnings]);
    }

    private static SectionValidation ValidateTimeline(IReadOnlyList<TimelinePhase> phases)
    {
        var warnings = new List<string>();

        if (phases.Count == 0)
        {
            warnings.Add("At least one timeline phase is required.");
        }
        else
        {
            var hasValidDuration = phases.Any(p => p.DurationWeeks > 0);
            if (!hasValidDuration)
                warnings.Add("At least one timeline phase must have a duration greater than 0 weeks.");
        }

        return new SectionValidation("Timeline", warnings.Count == 0, [.. warnings]);
    }
}

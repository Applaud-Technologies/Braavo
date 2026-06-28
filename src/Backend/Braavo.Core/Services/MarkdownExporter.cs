using System.Text;
using Braavo.Core.Entities;

namespace Braavo.Core.Services;

/// <summary>
/// Domain service that generates a full PRD Markdown document from structured product data.
/// </summary>
public static class MarkdownExporter
{
    public static string GeneratePrd(
        Product product,
        IReadOnlyList<Persona> personas,
        IReadOnlyList<UserStory> userStories,
        IReadOnlyList<Feature> features,
        IReadOnlyList<TimelinePhase> timelinePhases)
    {
        var sb = new StringBuilder();

        AppendOverview(sb, product);
        AppendPersonas(sb, personas);
        AppendUserStories(sb, userStories);
        AppendFeatures(sb, features);
        AppendTimeline(sb, timelinePhases);

        return sb.ToString();
    }

    private static void AppendOverview(StringBuilder sb, Product product)
    {
        sb.AppendLine($"# {product.Name} - Product Requirements Document");
        sb.AppendLine();
        sb.AppendLine("## Overview");
        sb.AppendLine();

        sb.AppendLine("### Vision");
        sb.AppendLine(string.IsNullOrWhiteSpace(product.Vision) ? "_Not defined yet._" : product.Vision);
        sb.AppendLine();

        sb.AppendLine("### Problem Statement");
        sb.AppendLine(string.IsNullOrWhiteSpace(product.ProblemStatement) ? "_Not defined yet._" : product.ProblemStatement);
        sb.AppendLine();

        sb.AppendLine("### Value Proposition");
        sb.AppendLine(string.IsNullOrWhiteSpace(product.ValueProposition) ? "_Not defined yet._" : product.ValueProposition);
        sb.AppendLine();

        if (product.TargetMarket.Length > 0)
        {
            sb.AppendLine("### Target Market");
            foreach (var segment in product.TargetMarket)
                sb.AppendLine($"- {segment}");
            sb.AppendLine();
        }

        if (product.BusinessGoals.Length > 0)
        {
            sb.AppendLine("### Business Goals");
            foreach (var goal in product.BusinessGoals)
                sb.AppendLine($"- {goal}");
            sb.AppendLine();
        }
    }

    private static void AppendPersonas(StringBuilder sb, IReadOnlyList<Persona> personas)
    {
        sb.AppendLine("## User Personas");
        sb.AppendLine();

        if (personas.Count == 0)
        {
            sb.AppendLine("_No personas defined yet._");
            sb.AppendLine();
            return;
        }

        foreach (var persona in personas.OrderBy(p => p.SortOrder))
        {
            sb.AppendLine($"### {persona.Name}");
            sb.AppendLine($"- **Role:** {persona.Role}");
            sb.AppendLine($"- **Technical Level:** {persona.TechnicalLevel}");

            if (persona.Goals.Length > 0)
            {
                sb.AppendLine("- **Goals:**");
                foreach (var goal in persona.Goals)
                    sb.AppendLine($"  - {goal}");
            }

            if (persona.PainPoints.Length > 0)
            {
                sb.AppendLine("- **Pain Points:**");
                foreach (var pain in persona.PainPoints)
                    sb.AppendLine($"  - {pain}");
            }

            if (persona.Motivations.Length > 0)
            {
                sb.AppendLine("- **Motivations:**");
                foreach (var motivation in persona.Motivations)
                    sb.AppendLine($"  - {motivation}");
            }

            if (!string.IsNullOrWhiteSpace(persona.Quote))
            {
                sb.AppendLine();
                sb.AppendLine($"> \"{persona.Quote}\"");
            }

            sb.AppendLine();
        }
    }

    private static void AppendUserStories(StringBuilder sb, IReadOnlyList<UserStory> userStories)
    {
        sb.AppendLine("## User Stories");
        sb.AppendLine();

        if (userStories.Count == 0)
        {
            sb.AppendLine("_No user stories defined yet._");
            sb.AppendLine();
            return;
        }

        // Group by priority
        var byPriority = userStories
            .OrderBy(s => s.Priority)
            .ThenBy(s => s.SortOrder)
            .GroupBy(s => s.Priority);

        foreach (var group in byPriority)
        {
            sb.AppendLine($"### {group.Key} Priority");
            sb.AppendLine();

            foreach (var story in group)
            {
                sb.AppendLine($"**As a** {story.AsA}, **I want** {story.IWant} **so that** {story.SoThat}");

                if (story.AcceptanceCriteria.Length > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("**Acceptance Criteria:**");
                    foreach (var criterion in story.AcceptanceCriteria)
                        sb.AppendLine($"- {criterion}");
                }

                sb.AppendLine();
            }
        }
    }

    private static void AppendFeatures(StringBuilder sb, IReadOnlyList<Feature> features)
    {
        sb.AppendLine("## Features");
        sb.AppendLine();

        if (features.Count == 0)
        {
            sb.AppendLine("_No features defined yet._");
            sb.AppendLine();
            return;
        }

        // Group by phase
        var byPhase = features
            .OrderBy(f => f.Phase)
            .ThenBy(f => f.SortOrder)
            .GroupBy(f => f.Phase);

        foreach (var group in byPhase)
        {
            var phaseLabel = group.Key switch
            {
                FeaturePhase.Mvp => "MVP",
                FeaturePhase.Enhanced => "Enhanced",
                FeaturePhase.Future => "Future",
                _ => group.Key.ToString()
            };

            sb.AppendLine($"### Phase: {phaseLabel}");
            sb.AppendLine();

            // Top-level features (no parent)
            var topLevel = group.Where(f => f.ParentId is null).ToList();
            var children = group.Where(f => f.ParentId is not null).ToList();

            foreach (var feature in topLevel)
            {
                AppendFeatureEntry(sb, feature, children, level: 4);
            }
        }
    }

    private static void AppendFeatureEntry(StringBuilder sb, Feature feature, List<Feature> allChildren, int level)
    {
        var heading = new string('#', Math.Min(level, 6));
        sb.AppendLine($"{heading} {feature.Name}");

        if (!string.IsNullOrWhiteSpace(feature.Description))
            sb.AppendLine(feature.Description);

        if (feature.Effort.HasValue)
            sb.AppendLine($"- **Effort:** {feature.Effort}");

        sb.AppendLine();

        var childFeatures = allChildren.Where(c => c.ParentId == feature.Id).OrderBy(c => c.SortOrder).ToList();
        foreach (var child in childFeatures)
            AppendFeatureEntry(sb, child, allChildren, level + 1);
    }

    private static void AppendTimeline(StringBuilder sb, IReadOnlyList<TimelinePhase> timelinePhases)
    {
        sb.AppendLine("## Timeline");
        sb.AppendLine();

        if (timelinePhases.Count == 0)
        {
            sb.AppendLine("_No timeline defined yet._");
            sb.AppendLine();
            return;
        }

        foreach (var phase in timelinePhases.OrderBy(p => p.SortOrder))
        {
            sb.AppendLine($"### {phase.Name}");
            sb.AppendLine($"- **Duration:** {phase.DurationWeeks} week{(phase.DurationWeeks == 1 ? "" : "s")}");

            if (phase.StartDate.HasValue)
                sb.AppendLine($"- **Start Date:** {phase.StartDate.Value:yyyy-MM-dd}");

            if (phase.Milestones.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("**Milestones:**");
                sb.AppendLine();
                sb.AppendLine("| Milestone | Week | Status | Deliverables |");
                sb.AppendLine("|-----------|------|--------|--------------|");
                foreach (var milestone in phase.Milestones.OrderBy(m => m.WeekNumber))
                {
                    var deliverables = milestone.Deliverables.Length > 0
                        ? string.Join(", ", milestone.Deliverables)
                        : "-";
                    sb.AppendLine($"| {milestone.Name} | {milestone.WeekNumber} | {milestone.Status} | {deliverables} |");
                }
            }

            sb.AppendLine();
        }

        sb.AppendLine($"---");
        sb.AppendLine($"_Generated by Braavo on {DateTime.UtcNow:yyyy-MM-dd}_");
    }

    public static string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Concat(name.Where(c => !invalid.Contains(c))).Replace(" ", "-").ToLowerInvariant();
    }
}

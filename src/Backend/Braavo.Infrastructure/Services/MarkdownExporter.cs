using Braavo.Core.Models;

namespace Braavo.Infrastructure.Services;

public static class MarkdownExporter
{
    public static string ExportUserStories(PrdContent prd)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("# User Stories");
        sb.AppendLine();
        sb.AppendLine($"*Exported from: {prd.Title}*");
        sb.AppendLine();

        for (int i = 0; i < prd.UserStories.Count; i++)
        {
            var story = prd.UserStories[i];
            sb.AppendLine($"## Story {i + 1}");
            sb.AppendLine();
            sb.AppendLine($"**As a** {story.AsA}");
            sb.AppendLine();
            sb.AppendLine($"**I want** {story.IWant}");
            sb.AppendLine();
            sb.AppendLine($"**So that** {story.SoThat}");
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public static string ExportFeatures(PrdContent prd)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("# Features");
        sb.AppendLine();
        sb.AppendLine($"*Exported from: {prd.Title}*");
        sb.AppendLine();

        for (int i = 0; i < prd.Features.Count; i++)
        {
            var feature = prd.Features[i];
            sb.AppendLine($"## {i + 1}. {feature.Name}");
            sb.AppendLine();
            sb.AppendLine(feature.Description);
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public static string ExportMetrics(PrdContent prd)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("# Success Metrics");
        sb.AppendLine();
        sb.AppendLine($"*Exported from: {prd.Title}*");
        sb.AppendLine();

        foreach (var metric in prd.SuccessMetrics)
        {
            sb.AppendLine($"- {metric}");
        }

        return sb.ToString();
    }
}

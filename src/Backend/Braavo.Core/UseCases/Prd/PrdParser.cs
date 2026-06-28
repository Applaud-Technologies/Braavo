using System.Text.RegularExpressions;
using Braavo.Core.Models;

namespace Braavo.Core.UseCases.Prd;

public static partial class PrdParser
{
    public static PrdContent Parse(string markdown)
    {
        var title = ExtractTitle(markdown);
        var overview = ExtractSection(markdown, "Overview");
        var problem = ExtractSection(markdown, "Problem Statement");
        var users = ExtractListItems(markdown, "Target Users");
        var features = ExtractFeatures(markdown);
        var metrics = ExtractListItems(markdown, "Success Metrics");
        var stories = ExtractUserStories(markdown);

        return new PrdContent(
            title,
            overview,
            problem,
            users,
            features,
            metrics,
            stories
        );
    }

    private static string ExtractTitle(string markdown)
    {
        var match = TitleRegex().Match(markdown);
        return match.Success ? match.Groups[1].Value.Trim() : "";
    }

    private static string ExtractSection(string markdown, string sectionName)
    {
        var pattern = $@"##\s*{sectionName}\s*\n([\s\S]*?)(?=\n##|\z)";
        var match = Regex.Match(markdown, pattern, RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Trim() : "";
    }

    private static List<string> ExtractListItems(string markdown, string sectionName)
    {
        var section = ExtractSection(markdown, sectionName);
        return ListItemRegex()
            .Matches(section)
            .Select(m => m.Groups[1].Value.Trim())
            .ToList();
    }

    private static List<PrdFeature> ExtractFeatures(string markdown)
    {
        var section = ExtractSection(markdown, "Features");
        var matches = FeatureRegex().Matches(section);

        return matches.Select(m => new PrdFeature(
            m.Groups[1].Value.Trim(),
            m.Groups[2].Value.Trim()
        )).ToList();
    }

    private static List<UserStory> ExtractUserStories(string markdown)
    {
        var section = ExtractSection(markdown, "User Stories");
        var matches = UserStoryRegex().Matches(section);

        return matches.Select(m => new UserStory(
            m.Groups[1].Value.Trim(),
            m.Groups[2].Value.Trim(),
            m.Groups[3].Value.Trim()
        )).ToList();
    }

    [GeneratedRegex(@"^#\s+(.+)$", RegexOptions.Multiline)]
    private static partial Regex TitleRegex();

    [GeneratedRegex(@"^[-*]\s+(.+)$", RegexOptions.Multiline)]
    private static partial Regex ListItemRegex();

    [GeneratedRegex(@"\*\*(.+?)\*\*\s*[-–:]\s*(.+?)(?=\n|$)", RegexOptions.Multiline)]
    private static partial Regex FeatureRegex();

    [GeneratedRegex(@"As\s+(?:a|an)\s+(.+?),\s*I\s+want\s+(?:to\s+)?(.+?)\s+so\s+that\s+(.+?)(?:\n|$)", RegexOptions.IgnoreCase)]
    private static partial Regex UserStoryRegex();
}

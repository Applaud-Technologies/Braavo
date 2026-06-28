namespace Braavo.Core.Models;

public record PrdContent(
    string Title,
    string Overview,
    string ProblemStatement,
    List<string> TargetUsers,
    List<PrdFeature> Features,
    List<string> SuccessMetrics,
    List<UserStory> UserStories
);

public record PrdFeature(string Name, string Description);

public record UserStory(string AsA, string IWant, string SoThat);

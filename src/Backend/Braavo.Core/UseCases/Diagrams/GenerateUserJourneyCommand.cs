using System.Text;
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Diagrams;

public record GenerateUserJourneyCommand(Guid ProductId, Guid UserId, Guid? PersonaId = null)
    : IRequest<GenerateUserJourneyResult>;

public record GenerateUserJourneyResult(
    bool Success,
    string? MermaidCode = null,
    string? Error = null,
    bool IsUnprocessableEntity = false
);

public class GenerateUserJourneyHandler : IRequestHandler<GenerateUserJourneyCommand, GenerateUserJourneyResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IPersonaRepository _personaRepo;
    private readonly IUserStoryRepository _storyRepo;

    public GenerateUserJourneyHandler(
        IProductRepository productRepo,
        IPersonaRepository personaRepo,
        IUserStoryRepository storyRepo)
    {
        _productRepo = productRepo;
        _personaRepo = personaRepo;
        _storyRepo = storyRepo;
    }

    public async Task<GenerateUserJourneyResult> Handle(GenerateUserJourneyCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new GenerateUserJourneyResult(false, Error: "Product not found");

        string actorLabel;
        IReadOnlyList<Entities.UserStory> stories;

        if (request.PersonaId.HasValue)
        {
            var persona = await _personaRepo.GetByIdAsync(request.PersonaId.Value, ct);
            if (persona is null || persona.ProductId != request.ProductId)
                return new GenerateUserJourneyResult(false, Error: "Persona not found");

            actorLabel = $"{persona.Name} ({persona.Role})";
            stories = await _storyRepo.GetByPersonaIdAsync(request.PersonaId.Value, ct);
        }
        else
        {
            actorLabel = product.Name + " User";
            stories = await _storyRepo.GetByProductIdAsync(request.ProductId, ct);
        }

        if (stories.Count == 0)
            return new GenerateUserJourneyResult(false, Error: "No user stories found", IsUnprocessableEntity: true);

        var orderedStories = stories.OrderBy(s => s.SortOrder).ThenBy(s => s.CreatedAt).ToList();
        var mermaidCode = GenerateMermaidUserJourney(actorLabel, orderedStories);

        return new GenerateUserJourneyResult(true, MermaidCode: mermaidCode);
    }

    private static string GenerateMermaidUserJourney(
        string actorLabel,
        List<Entities.UserStory> stories)
    {
        var sb = new StringBuilder();
        sb.AppendLine("flowchart LR");

        // Emit nodes
        for (var i = 0; i < stories.Count; i++)
        {
            var label = EscapeLabel(stories[i].IWant);
            sb.AppendLine($"    s{i}[\"{label}\"]");
        }

        // Emit edges
        for (var i = 0; i < stories.Count - 1; i++)
        {
            sb.AppendLine($"    s{i} --> s{i + 1}");
        }

        // Attach actor note if more than one step
        if (stories.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"    %% Actor: {EscapeComment(actorLabel)}");
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>Escapes double-quotes and strips characters that break Mermaid node labels.</summary>
    private static string EscapeLabel(string text)
        => text.Replace("\"", "'").Replace("\n", " ").Replace("\r", "");

    private static string EscapeComment(string text)
        => text.Replace("\n", " ").Replace("\r", "");
}

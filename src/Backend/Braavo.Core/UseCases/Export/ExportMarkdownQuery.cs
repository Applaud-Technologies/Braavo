using Braavo.Core.Interfaces;
using Braavo.Core.Services;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Export;

public record ExportMarkdownQuery(Guid ProductId, Guid UserId) : IRequest<ExportMarkdownResult>;

public record ExportMarkdownResult(
    bool Success,
    byte[]? Content = null,
    string? FileName = null,
    string? Error = null
);

public class ExportMarkdownHandler : IRequestHandler<ExportMarkdownQuery, ExportMarkdownResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IPersonaRepository _personaRepo;
    private readonly IUserStoryRepository _userStoryRepo;
    private readonly IFeatureRepository _featureRepo;
    private readonly ITimelineRepository _timelineRepo;

    public ExportMarkdownHandler(
        IProductRepository productRepo,
        IPersonaRepository personaRepo,
        IUserStoryRepository userStoryRepo,
        IFeatureRepository featureRepo,
        ITimelineRepository timelineRepo)
    {
        _productRepo = productRepo;
        _personaRepo = personaRepo;
        _userStoryRepo = userStoryRepo;
        _featureRepo = featureRepo;
        _timelineRepo = timelineRepo;
    }

    public async Task<ExportMarkdownResult> Handle(ExportMarkdownQuery request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null)
            return new ExportMarkdownResult(false, Error: "Product not found");

        if (product.OwnerId != UserId.From(request.UserId))
            return new ExportMarkdownResult(false, Error: "Forbidden");

        var personas = await _personaRepo.GetByProductIdAsync(request.ProductId, ct);
        var userStories = await _userStoryRepo.GetByProductIdAsync(request.ProductId, ct);
        var features = await _featureRepo.GetByProductIdAsync(request.ProductId, ct);
        var timelinePhases = await _timelineRepo.GetByProductIdAsync(request.ProductId, ct);

        var markdown = MarkdownExporter.GeneratePrd(product, personas, userStories, features, timelinePhases);

        var content = System.Text.Encoding.UTF8.GetBytes(markdown);
        var fileName = $"{MarkdownExporter.SanitizeFileName(product.Name)}-prd.md";

        return new ExportMarkdownResult(true, content, fileName);
    }
}

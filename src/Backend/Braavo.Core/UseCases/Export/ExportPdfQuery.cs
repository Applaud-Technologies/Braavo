using Braavo.Core.Interfaces;
using Braavo.Core.Services;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Export;

public record ExportPdfQuery(Guid ProductId, Guid UserId) : IRequest<ExportPdfResult>;

public record ExportPdfResult(
    bool Success,
    byte[]? Content = null,
    string? FileName = null,
    string? Error = null
);

public class ExportPdfHandler : IRequestHandler<ExportPdfQuery, ExportPdfResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IPersonaRepository _personaRepo;
    private readonly IUserStoryRepository _userStoryRepo;
    private readonly IFeatureRepository _featureRepo;
    private readonly ITimelineRepository _timelineRepo;

    public ExportPdfHandler(
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

    public async Task<ExportPdfResult> Handle(ExportPdfQuery request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null)
            return new ExportPdfResult(false, Error: "Product not found");

        if (product.OwnerId != UserId.From(request.UserId))
            return new ExportPdfResult(false, Error: "Forbidden");

        var personas = await _personaRepo.GetByProductIdAsync(request.ProductId, ct);
        var userStories = await _userStoryRepo.GetByProductIdAsync(request.ProductId, ct);
        var features = await _featureRepo.GetByProductIdAsync(request.ProductId, ct);
        var timelinePhases = await _timelineRepo.GetByProductIdAsync(request.ProductId, ct);

        var pdfBytes = PdfExporter.GeneratePrd(product, personas, userStories, features, timelinePhases);

        var fileName = $"{MarkdownExporter.SanitizeFileName(product.Name)}-prd.pdf";

        return new ExportPdfResult(true, pdfBytes, fileName);
    }
}

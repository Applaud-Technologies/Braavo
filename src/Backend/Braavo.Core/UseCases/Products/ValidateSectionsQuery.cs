using Braavo.Core.Interfaces;
using Braavo.Core.Services;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Products;

public record ValidateSectionsQuery(Guid ProductId, Guid UserId)
    : IRequest<ValidateSectionsResponse?>;

public record ValidateSectionsResponse(
    IReadOnlyList<SectionValidationDto> Validations);

public record SectionValidationDto(
    string Section,
    bool IsValid,
    string[] Warnings);

public class ValidateSectionsHandler : IRequestHandler<ValidateSectionsQuery, ValidateSectionsResponse?>
{
    private readonly IProductRepository   _productRepo;
    private readonly IPersonaRepository   _personaRepo;
    private readonly IUserStoryRepository _storyRepo;
    private readonly IFeatureRepository   _featureRepo;
    private readonly ITimelineRepository  _timelineRepo;

    public ValidateSectionsHandler(
        IProductRepository   productRepo,
        IPersonaRepository   personaRepo,
        IUserStoryRepository storyRepo,
        IFeatureRepository   featureRepo,
        ITimelineRepository  timelineRepo)
    {
        _productRepo  = productRepo;
        _personaRepo  = personaRepo;
        _storyRepo    = storyRepo;
        _featureRepo  = featureRepo;
        _timelineRepo = timelineRepo;
    }

    public async Task<ValidateSectionsResponse?> Handle(
        ValidateSectionsQuery request,
        CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return null;

        var validations = await SectionValidator.ValidateAsync(
            product.Id,
            product.Vision,
            product.ProblemStatement,
            _personaRepo,
            _storyRepo,
            _featureRepo,
            _timelineRepo,
            ct);

        var dtos = validations
            .Select(v => new SectionValidationDto(v.Section, v.IsValid, v.Warnings))
            .ToList()
            .AsReadOnly();

        return new ValidateSectionsResponse(dtos);
    }
}

using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Products;

public record GetDependencyStatusQuery(Guid ProductId, Guid UserId) : IRequest<DependencyStatusResult>;

public record DependencyStatusResult(
    bool Success,
    bool HasPersonas = false,
    bool HasFeatures = false,
    bool HasStories = false,
    bool CanCreateFeatures = false,
    bool CanCreateStories = false,
    string? Error = null
);

public class GetDependencyStatusHandler : IRequestHandler<GetDependencyStatusQuery, DependencyStatusResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IPersonaRepository _personaRepo;
    private readonly IFeatureRepository _featureRepo;
    private readonly IUserStoryRepository _storyRepo;

    public GetDependencyStatusHandler(
        IProductRepository productRepo,
        IPersonaRepository personaRepo,
        IFeatureRepository featureRepo,
        IUserStoryRepository storyRepo)
    {
        _productRepo = productRepo;
        _personaRepo = personaRepo;
        _featureRepo = featureRepo;
        _storyRepo = storyRepo;
    }

    public async Task<DependencyStatusResult> Handle(GetDependencyStatusQuery request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new DependencyStatusResult(false, Error: "Product not found");

        var personas = await _personaRepo.GetByProductIdAsync(request.ProductId, ct);
        var features = await _featureRepo.GetByProductIdAsync(request.ProductId, ct);
        var stories = await _storyRepo.GetByProductIdAsync(request.ProductId, ct);

        var hasPersonas = personas.Count > 0;
        var hasFeatures = features.Count > 0;
        var hasStories = stories.Count > 0;

        return new DependencyStatusResult(
            Success: true,
            HasPersonas: hasPersonas,
            HasFeatures: hasFeatures,
            HasStories: hasStories,
            CanCreateFeatures: hasPersonas,
            CanCreateStories: hasFeatures
        );
    }
}

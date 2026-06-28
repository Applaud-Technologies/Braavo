using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Features;

public record ListFeaturesQuery(Guid ProductId, Guid UserId) : IRequest<ListFeaturesResult>;

public record ListFeaturesResult(
    bool Success,
    IReadOnlyList<FeatureDto>? Features = null,
    string? Error = null
);

public record FeatureDto(
    Guid Id,
    Guid ProductId,
    Guid? ParentId,
    string Name,
    string Description,
    string Phase,
    string? Effort,
    Guid[] LinkedStoryIds,
    int SortOrder,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public class ListFeaturesHandler : IRequestHandler<ListFeaturesQuery, ListFeaturesResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IFeatureRepository _featureRepo;

    public ListFeaturesHandler(IProductRepository productRepo, IFeatureRepository featureRepo)
    {
        _productRepo = productRepo;
        _featureRepo = featureRepo;
    }

    public async Task<ListFeaturesResult> Handle(ListFeaturesQuery request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new ListFeaturesResult(false, Error: "Product not found");

        var features = await _featureRepo.GetByProductIdAsync(request.ProductId, ct);

        var dtos = features.Select(f => new FeatureDto(
            f.Id, f.ProductId, f.ParentId,
            f.Name, f.Description,
            f.Phase.ToString(),
            f.Effort?.ToString(),
            f.LinkedStoryIds,
            f.SortOrder,
            f.CreatedAt, f.UpdatedAt
        )).ToList();

        return new ListFeaturesResult(true, dtos);
    }
}

using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Features;

public record UpdateFeatureCommand(
    Guid FeatureId,
    Guid ProductId,
    Guid UserId,
    string Name,
    string Description,
    EffortSize? Effort = null,
    Guid? ParentId = null,
    int? SortOrder = null
) : IRequest<UpdateFeatureResult>;

public record UpdateFeatureResult(bool Success, string? Error = null);

public class UpdateFeatureHandler : IRequestHandler<UpdateFeatureCommand, UpdateFeatureResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IFeatureRepository _featureRepo;

    public UpdateFeatureHandler(IProductRepository productRepo, IFeatureRepository featureRepo)
    {
        _productRepo = productRepo;
        _featureRepo = featureRepo;
    }

    public async Task<UpdateFeatureResult> Handle(UpdateFeatureCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new UpdateFeatureResult(false, "Product not found");

        var feature = await _featureRepo.GetByIdAsync(request.FeatureId, ct);
        if (feature is null || feature.ProductId != request.ProductId)
            return new UpdateFeatureResult(false, "Feature not found");

        if (string.IsNullOrWhiteSpace(request.Name))
            return new UpdateFeatureResult(false, "Feature name is required");

        if (string.IsNullOrWhiteSpace(request.Description))
            return new UpdateFeatureResult(false, "Feature description is required");

        feature.Update(request.Name, request.Description, request.Effort);
        feature.SetParent(request.ParentId);

        if (request.SortOrder.HasValue)
            feature.UpdateSortOrder(request.SortOrder.Value);

        await _featureRepo.UpdateAsync(feature, ct);

        return new UpdateFeatureResult(true);
    }
}

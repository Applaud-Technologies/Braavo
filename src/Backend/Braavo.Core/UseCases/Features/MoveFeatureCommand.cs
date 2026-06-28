using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Features;

public record MoveFeatureCommand(
    Guid FeatureId,
    Guid ProductId,
    Guid UserId,
    FeaturePhase NewPhase,
    int? NewSortOrder = null
) : IRequest<MoveFeatureResult>;

public record MoveFeatureResult(bool Success, string? Error = null);

public class MoveFeatureHandler : IRequestHandler<MoveFeatureCommand, MoveFeatureResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IFeatureRepository _featureRepo;

    public MoveFeatureHandler(IProductRepository productRepo, IFeatureRepository featureRepo)
    {
        _productRepo = productRepo;
        _featureRepo = featureRepo;
    }

    public async Task<MoveFeatureResult> Handle(MoveFeatureCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new MoveFeatureResult(false, "Product not found");

        var feature = await _featureRepo.GetByIdAsync(request.FeatureId, ct);
        if (feature is null || feature.ProductId != request.ProductId)
            return new MoveFeatureResult(false, "Feature not found");

        feature.ChangePhase(request.NewPhase);

        if (request.NewSortOrder.HasValue)
            feature.UpdateSortOrder(request.NewSortOrder.Value);

        await _featureRepo.UpdateAsync(feature, ct);

        return new MoveFeatureResult(true);
    }
}

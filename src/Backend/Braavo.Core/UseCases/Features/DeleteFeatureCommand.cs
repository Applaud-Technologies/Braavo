using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Features;

public record DeleteFeatureCommand(
    Guid FeatureId,
    Guid ProductId,
    Guid UserId
) : IRequest<DeleteFeatureResult>;

public record DeleteFeatureResult(bool Success, string? Error = null);

public class DeleteFeatureHandler : IRequestHandler<DeleteFeatureCommand, DeleteFeatureResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IFeatureRepository _featureRepo;

    public DeleteFeatureHandler(IProductRepository productRepo, IFeatureRepository featureRepo)
    {
        _productRepo = productRepo;
        _featureRepo = featureRepo;
    }

    public async Task<DeleteFeatureResult> Handle(DeleteFeatureCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new DeleteFeatureResult(false, "Product not found");

        var feature = await _featureRepo.GetByIdAsync(request.FeatureId, ct);
        if (feature is null || feature.ProductId != request.ProductId)
            return new DeleteFeatureResult(false, "Feature not found");

        await _featureRepo.DeleteAsync(request.FeatureId, ct);

        return new DeleteFeatureResult(true);
    }
}

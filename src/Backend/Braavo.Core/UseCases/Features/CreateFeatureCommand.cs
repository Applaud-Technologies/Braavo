using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Products;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Features;

public record CreateFeatureCommand(
    Guid ProductId,
    Guid UserId,
    string Name,
    string Description,
    FeaturePhase Phase = FeaturePhase.Mvp,
    EffortSize? Effort = null,
    Guid? ParentId = null,
    int SortOrder = 0
) : IRequest<CreateFeatureResult>;

public record CreateFeatureResult(
    Guid FeatureId,
    bool Success,
    string? Error = null
);

public class CreateFeatureHandler : IRequestHandler<CreateFeatureCommand, CreateFeatureResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IFeatureRepository _featureRepo;
    private readonly IPersonaRepository _personaRepo;
    private readonly IMediator _mediator;

    public CreateFeatureHandler(
        IProductRepository productRepo,
        IFeatureRepository featureRepo,
        IPersonaRepository personaRepo,
        IMediator mediator)
    {
        _productRepo = productRepo;
        _featureRepo = featureRepo;
        _personaRepo = personaRepo;
        _mediator    = mediator;
    }

    public async Task<CreateFeatureResult> Handle(CreateFeatureCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new CreateFeatureResult(Guid.Empty, false, "Product not found");

        var personas = await _personaRepo.GetByProductIdAsync(request.ProductId, ct);
        if (personas.Count == 0)
            return new CreateFeatureResult(Guid.Empty, false, "At least one persona is required before creating features");

        if (string.IsNullOrWhiteSpace(request.Name))
            return new CreateFeatureResult(Guid.Empty, false, "Feature name is required");

        if (string.IsNullOrWhiteSpace(request.Description))
            return new CreateFeatureResult(Guid.Empty, false, "Feature description is required");

        var feature = Feature.Create(request.ProductId, request.Name, request.Description);
        feature.ChangePhase(request.Phase);
        feature.Update(request.Name, request.Description, request.Effort);
        feature.UpdateSortOrder(request.SortOrder);

        if (request.ParentId.HasValue)
            feature.SetParent(request.ParentId);

        await _featureRepo.AddAsync(feature, ct);

        await _mediator.Send(new RecalculateCompletionCommand(request.ProductId), ct);

        return new CreateFeatureResult(feature.Id, true);
    }
}

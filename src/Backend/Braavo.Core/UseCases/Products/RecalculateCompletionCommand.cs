using Braavo.Core.Interfaces;
using Braavo.Core.Services;
using MediatR;

namespace Braavo.Core.UseCases.Products;

/// <summary>
/// Recalculates a product's completion percentage and persists the result.
/// Call this from any handler that modifies a PRD section.
/// </summary>
public record RecalculateCompletionCommand(Guid ProductId) : IRequest<RecalculateCompletionResponse>;

public record RecalculateCompletionResponse(bool Success, int CompletionPercentage, string? Error = null);

public class RecalculateCompletionHandler : IRequestHandler<RecalculateCompletionCommand, RecalculateCompletionResponse>
{
    private readonly IProductRepository _productRepo;
    private readonly IPersonaRepository _personaRepo;
    private readonly IUserStoryRepository _userStoryRepo;
    private readonly IFeatureRepository _featureRepo;
    private readonly ITimelineRepository _timelineRepo;

    public RecalculateCompletionHandler(
        IProductRepository productRepo,
        IPersonaRepository personaRepo,
        IUserStoryRepository userStoryRepo,
        IFeatureRepository featureRepo,
        ITimelineRepository timelineRepo)
    {
        _productRepo   = productRepo;
        _personaRepo   = personaRepo;
        _userStoryRepo = userStoryRepo;
        _featureRepo   = featureRepo;
        _timelineRepo  = timelineRepo;
    }

    public async Task<RecalculateCompletionResponse> Handle(
        RecalculateCompletionCommand request,
        CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null)
            return new RecalculateCompletionResponse(false, 0, $"Product {request.ProductId} not found");

        var percentage = await CompletionCalculator.CalculateAsync(
            productId:          product.Id,
            vision:             product.Vision,
            problemStatement:   product.ProblemStatement,
            valueProposition:   product.ValueProposition,
            targetMarket:       product.TargetMarket,
            personaRepo:        _personaRepo,
            userStoryRepo:      _userStoryRepo,
            featureRepo:        _featureRepo,
            timelineRepo:       _timelineRepo,
            ct:                 ct);

        product.UpdateCompletionPercentage(percentage);
        await _productRepo.UpdateAsync(product, ct);

        return new RecalculateCompletionResponse(true, percentage);
    }
}

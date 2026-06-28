using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Products;

public record UpdateOverviewCommand(
    Guid ProductId,
    Guid UserId,
    string Vision,
    string ProblemStatement,
    string ValueProposition,
    string[]? TargetMarket = null,
    string[]? BusinessGoals = null
) : IRequest<UpdateOverviewResult>;

public record UpdateOverviewResult(bool Success, string? Error = null);

public class UpdateOverviewHandler : IRequestHandler<UpdateOverviewCommand, UpdateOverviewResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IMediator _mediator;

    public UpdateOverviewHandler(IProductRepository productRepo, IMediator mediator)
    {
        _productRepo = productRepo;
        _mediator    = mediator;
    }

    public async Task<UpdateOverviewResult> Handle(UpdateOverviewCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new UpdateOverviewResult(false, "Product not found");

        product.UpdateOverview(request.Vision, request.ProblemStatement, request.ValueProposition);

        if (request.TargetMarket is { Length: > 0 })
            product.UpdateTargetMarket(request.TargetMarket);

        if (request.BusinessGoals is { Length: > 0 })
            product.UpdateBusinessGoals(request.BusinessGoals);

        await _productRepo.UpdateAsync(product, ct);

        await _mediator.Send(new RecalculateCompletionCommand(request.ProductId), ct);

        return new UpdateOverviewResult(true);
    }
}

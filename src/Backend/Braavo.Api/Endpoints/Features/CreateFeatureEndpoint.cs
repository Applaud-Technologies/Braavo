using System.Security.Claims;
using Braavo.Core.Entities;
using Braavo.Core.UseCases.Features;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Features;

public record CreateFeatureRequest(
    string Name,
    string Description,
    FeaturePhase Phase = FeaturePhase.Mvp,
    EffortSize? Effort = null,
    Guid? ParentId = null,
    int SortOrder = 0
);

public class CreateFeatureEndpoint : Endpoint<CreateFeatureRequest, CreateFeatureResult>
{
    private readonly IMediator _mediator;

    public CreateFeatureEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/products/{productId}/features");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CreateFeatureRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var productId = Route<Guid>("productId");

        var command = new CreateFeatureCommand(
            productId, userId,
            req.Name, req.Description,
            req.Phase, req.Effort,
            req.ParentId, req.SortOrder
        );
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            await SendAsync(result, 400, ct);
            return;
        }

        await SendCreatedAtAsync<ListFeaturesEndpoint>(
            new { productId },
            result,
            cancellation: ct
        );
    }
}

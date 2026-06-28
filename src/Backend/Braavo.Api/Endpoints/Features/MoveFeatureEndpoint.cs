using System.Security.Claims;
using Braavo.Core.Entities;
using Braavo.Core.UseCases.Features;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Features;

public record MoveFeatureRequest(
    FeaturePhase NewPhase,
    int? NewSortOrder = null
);

public class MoveFeatureEndpoint : Endpoint<MoveFeatureRequest, MoveFeatureResult>
{
    private readonly IMediator _mediator;

    public MoveFeatureEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/products/{productId}/features/{id}/move");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(MoveFeatureRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var productId = Route<Guid>("productId");
        var featureId = Route<Guid>("id");

        var command = new MoveFeatureCommand(
            featureId, productId, userId,
            req.NewPhase, req.NewSortOrder
        );
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            await SendAsync(result, 400, ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}

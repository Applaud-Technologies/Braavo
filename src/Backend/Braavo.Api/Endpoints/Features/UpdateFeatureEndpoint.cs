using System.Security.Claims;
using Braavo.Core.Entities;
using Braavo.Core.UseCases.Features;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Features;

public record UpdateFeatureRequest(
    string Name,
    string Description,
    EffortSize? Effort = null,
    Guid? ParentId = null,
    int? SortOrder = null
);

public class UpdateFeatureEndpoint : Endpoint<UpdateFeatureRequest, UpdateFeatureResult>
{
    private readonly IMediator _mediator;

    public UpdateFeatureEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/products/{productId}/features/{id}");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(UpdateFeatureRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var productId = Route<Guid>("productId");
        var featureId = Route<Guid>("id");

        var command = new UpdateFeatureCommand(
            featureId, productId, userId,
            req.Name, req.Description,
            req.Effort, req.ParentId, req.SortOrder
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

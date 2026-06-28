using System.Security.Claims;
using Braavo.Core.UseCases.Features;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Features;

public class DeleteFeatureEndpoint : EndpointWithoutRequest<DeleteFeatureResult>
{
    private readonly IMediator _mediator;

    public DeleteFeatureEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("/api/products/{productId}/features/{id}");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var productId = Route<Guid>("productId");
        var featureId = Route<Guid>("id");

        var command = new DeleteFeatureCommand(featureId, productId, userId);
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            await SendAsync(result, 404, ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}

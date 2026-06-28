using System.Security.Claims;
using Braavo.Core.UseCases.Timeline;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Timeline;

public class GetTimelineEndpoint : EndpointWithoutRequest<GetTimelineResult>
{
    private readonly IMediator _mediator;

    public GetTimelineEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/products/{productId}/timeline");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var productId = Route<Guid>("productId");

        var result = await _mediator.Send(new GetTimelineQuery(productId, userId), ct);

        if (!result.Success)
        {
            await SendAsync(result, 404, ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}

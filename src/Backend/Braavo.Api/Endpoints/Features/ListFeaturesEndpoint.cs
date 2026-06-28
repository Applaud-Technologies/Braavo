using System.Security.Claims;
using Braavo.Core.UseCases.Features;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Features;

public class ListFeaturesEndpoint : EndpointWithoutRequest<ListFeaturesResult>
{
    private readonly IMediator _mediator;

    public ListFeaturesEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/products/{productId}/features");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var productId = Route<Guid>("productId");

        var result = await _mediator.Send(new ListFeaturesQuery(productId, userId), ct);

        if (!result.Success)
        {
            await SendAsync(result, 404, ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}

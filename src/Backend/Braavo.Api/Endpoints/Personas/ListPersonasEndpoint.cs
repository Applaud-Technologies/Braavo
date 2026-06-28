using System.Security.Claims;
using Braavo.Core.UseCases.Personas;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Personas;

public class ListPersonasEndpoint : EndpointWithoutRequest<ListPersonasResult>
{
    private readonly IMediator _mediator;

    public ListPersonasEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/products/{productId}/personas");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var productId = Route<Guid>("productId");

        var result = await _mediator.Send(new ListPersonasQuery(productId, userId), ct);

        if (!result.Success)
        {
            await SendAsync(result, 404, ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}

using System.Security.Claims;
using Braavo.Core.UseCases.Diagrams;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Diagrams;

public class GenerateGanttEndpoint : EndpointWithoutRequest<GenerateGanttResult>
{
    private readonly IMediator _mediator;

    public GenerateGanttEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/products/{productId}/diagrams/gantt");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var productId = Route<Guid>("productId");

        var command = new GenerateGanttCommand(productId, userId);
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            var statusCode = result.Error == "No timeline phases found" ? 422 : 404;
            await SendAsync(result, statusCode, ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}

using System.Security.Claims;
using Braavo.Core.UseCases.Diagrams;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Diagrams;

public class GenerateFeatureHierarchyEndpoint : EndpointWithoutRequest<GenerateFeatureHierarchyResult>
{
    private readonly IMediator _mediator;

    public GenerateFeatureHierarchyEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/products/{productId}/diagrams/feature-hierarchy");
        Policies("Authenticated");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdStr is null || !Guid.TryParse(userIdStr, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var productId = Route<Guid>("productId");

        var command = new GenerateFeatureHierarchyCommand(productId, userId);
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            var statusCode = result.IsUnprocessableEntity ? 422 : 404;
            await SendAsync(result, statusCode, ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}

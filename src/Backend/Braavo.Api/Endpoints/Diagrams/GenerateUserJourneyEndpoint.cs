using System.Security.Claims;
using Braavo.Core.UseCases.Diagrams;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Diagrams;

public record GenerateUserJourneyRequest(Guid? PersonaId = null);

public class GenerateUserJourneyEndpoint : Endpoint<GenerateUserJourneyRequest, GenerateUserJourneyResult>
{
    private readonly IMediator _mediator;

    public GenerateUserJourneyEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/products/{productId}/diagrams/user-journey");
        Policies("Authenticated");
    }

    public override async Task HandleAsync(GenerateUserJourneyRequest req, CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdStr is null || !Guid.TryParse(userIdStr, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var productId = Route<Guid>("productId");

        var command = new GenerateUserJourneyCommand(productId, userId, req.PersonaId);
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

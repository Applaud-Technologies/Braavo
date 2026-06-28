using System.Security.Claims;
using Braavo.Core.UseCases.AI;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.AI;

public record GeneratePersonaRequest(Guid ProductId, string Description);

public class GeneratePersonaEndpoint : Endpoint<GeneratePersonaRequest, GeneratePersonaResult>
{
    private readonly IMediator _mediator;

    public GeneratePersonaEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/ai/generate-persona");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(GeneratePersonaRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new GeneratePersonaCommand(req.ProductId, userId, req.Description);
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            await SendAsync(result, 400, ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}

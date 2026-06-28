using System.Security.Claims;
using Braavo.Core.UseCases.AI;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.AI;

public record RefineContentRequest(string Content, string ContentType, string? Instruction = null);

public class RefineContentEndpoint : Endpoint<RefineContentRequest, RefineContentResult>
{
    private readonly IMediator _mediator;

    public RefineContentEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/ai/refine-content");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(RefineContentRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new RefineContentCommand(userId, req.Content, req.ContentType, req.Instruction);
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            await SendAsync(result, 400, ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}

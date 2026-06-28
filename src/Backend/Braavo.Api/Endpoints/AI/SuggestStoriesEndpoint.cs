using System.Security.Claims;
using Braavo.Core.UseCases.AI;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.AI;

public record SuggestStoriesRequest(Guid ProductId, Guid PersonaId, string? AdditionalContext = null);

public class SuggestStoriesEndpoint : Endpoint<SuggestStoriesRequest, SuggestStoriesResult>
{
    private readonly IMediator _mediator;

    public SuggestStoriesEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/ai/suggest-stories");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(SuggestStoriesRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new SuggestStoriesCommand(req.ProductId, userId, req.PersonaId, req.AdditionalContext);
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            await SendAsync(result, 400, ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}

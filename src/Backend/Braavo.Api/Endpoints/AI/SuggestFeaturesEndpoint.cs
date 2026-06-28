using System.Security.Claims;
using Braavo.Core.UseCases.AI;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.AI;

public record SuggestFeaturesRequest(Guid ProductId, Guid[]? StoryIds = null);

public class SuggestFeaturesEndpoint : Endpoint<SuggestFeaturesRequest, SuggestFeaturesResult>
{
    private readonly IMediator _mediator;

    public SuggestFeaturesEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/ai/suggest-features");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(SuggestFeaturesRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new SuggestFeaturesCommand(req.ProductId, userId, req.StoryIds);
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            await SendAsync(result, 400, ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}

using System.Security.Claims;
using Braavo.Core.Entities;
using Braavo.Core.UseCases.UserStories;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.UserStories;

public record UpdateUserStoryRequest(
    string AsA,
    string IWant,
    string SoThat,
    StoryPriority Priority,
    string[] AcceptanceCriteria,
    Guid? PersonaId = null
);

public class UpdateUserStoryEndpoint : Endpoint<UpdateUserStoryRequest, UpdateUserStoryResult>
{
    private readonly IMediator _mediator;

    public UpdateUserStoryEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/products/{productId}/stories/{id}");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(UpdateUserStoryRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var productId = Route<Guid>("productId");
        var storyId = Route<Guid>("id");

        var command = new UpdateUserStoryCommand(
            storyId, productId, userId,
            req.AsA, req.IWant, req.SoThat,
            req.Priority, req.AcceptanceCriteria, req.PersonaId
        );
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            await SendAsync(result, 400, ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}

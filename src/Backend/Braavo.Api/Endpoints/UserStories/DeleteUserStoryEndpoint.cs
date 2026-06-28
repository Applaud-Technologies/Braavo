using System.Security.Claims;
using Braavo.Core.UseCases.UserStories;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.UserStories;

public class DeleteUserStoryEndpoint : EndpointWithoutRequest<DeleteUserStoryResult>
{
    private readonly IMediator _mediator;

    public DeleteUserStoryEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("/api/products/{productId}/stories/{id}");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var productId = Route<Guid>("productId");
        var storyId = Route<Guid>("id");

        var command = new DeleteUserStoryCommand(storyId, productId, userId);
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            await SendAsync(result, 404, ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}

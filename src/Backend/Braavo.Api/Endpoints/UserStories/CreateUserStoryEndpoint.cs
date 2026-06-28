using System.Security.Claims;
using Braavo.Core.UseCases.UserStories;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.UserStories;

public record CreateUserStoryRequest(
    string AsA,
    string IWant,
    string SoThat,
    Guid? PersonaId = null
);

public class CreateUserStoryEndpoint : Endpoint<CreateUserStoryRequest, CreateUserStoryResult>
{
    private readonly IMediator _mediator;

    public CreateUserStoryEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/products/{productId}/stories");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CreateUserStoryRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var productId = Route<Guid>("productId");

        var command = new CreateUserStoryCommand(productId, userId, req.AsA, req.IWant, req.SoThat, req.PersonaId);
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            await SendAsync(result, 400, ct);
            return;
        }

        await SendCreatedAtAsync<ListUserStoriesEndpoint>(
            new { productId },
            result,
            cancellation: ct
        );
    }
}

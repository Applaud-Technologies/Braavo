using System.Security.Claims;
using Braavo.Core.UseCases.UserStories;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.UserStories;

public class ListUserStoriesEndpoint : EndpointWithoutRequest<ListUserStoriesResult>
{
    private readonly IMediator _mediator;

    public ListUserStoriesEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/products/{productId}/stories");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var productId = Route<Guid>("productId");

        var result = await _mediator.Send(new ListUserStoriesQuery(productId, userId), ct);

        if (!result.Success)
        {
            await SendAsync(result, 404, ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}

using System.Security.Claims;
using Braavo.Core.UseCases.Products;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Products;

public class GetDependencyStatusEndpoint : EndpointWithoutRequest<DependencyStatusResponse>
{
    private readonly IMediator _mediator;

    public GetDependencyStatusEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Get("/products/{id}/dependencies");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var productId = Route<Guid>("id");
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _mediator.Send(new GetDependencyStatusQuery(productId, userId), ct);

        if (!result.Success)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(new DependencyStatusResponse(
            result.HasPersonas,
            result.HasFeatures,
            result.HasStories,
            result.CanCreateFeatures,
            result.CanCreateStories
        ), ct);
    }
}

public record DependencyStatusResponse(
    bool HasPersonas,
    bool HasFeatures,
    bool HasStories,
    bool CanCreateFeatures,
    bool CanCreateStories
);

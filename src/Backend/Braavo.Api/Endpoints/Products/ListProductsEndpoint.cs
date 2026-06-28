using System.Security.Claims;
using Braavo.Core.UseCases.Products;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Products;

public class ListProductsEndpoint : EndpointWithoutRequest<IReadOnlyList<ProductSummaryDto>>
{
    private readonly IMediator _mediator;

    public ListProductsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/products");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new ListProductsQuery(userId), ct);
        await SendOkAsync(result, ct);
    }
}

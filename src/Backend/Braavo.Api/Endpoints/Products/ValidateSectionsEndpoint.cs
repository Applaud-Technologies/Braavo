using System.Security.Claims;
using Braavo.Core.UseCases.Products;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Products;

public class ValidateSectionsEndpoint : EndpointWithoutRequest<ValidateSectionsResponse>
{
    private readonly IMediator _mediator;

    public ValidateSectionsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/products/{id}/validate");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var productId = Route<Guid>("id");
        var userId    = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _mediator.Send(new ValidateSectionsQuery(productId, userId), ct);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}

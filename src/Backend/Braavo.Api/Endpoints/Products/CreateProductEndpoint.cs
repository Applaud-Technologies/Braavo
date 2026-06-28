using System.Security.Claims;
using Braavo.Core.UseCases.Products;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Products;

public record CreateProductRequest(string Name, string Description, string[]? Categories);

public class CreateProductEndpoint : Endpoint<CreateProductRequest, CreateProductResponse>
{
    private readonly IMediator _mediator;

    public CreateProductEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/products");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new CreateProductCommand(req.Name, req.Description, userId, req.Categories);
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            await SendAsync(result, 400, ct);
            return;
        }

        await SendCreatedAtAsync<GetProductEndpoint>(
            new { id = result.ProductId },
            result,
            cancellation: ct
        );
    }
}

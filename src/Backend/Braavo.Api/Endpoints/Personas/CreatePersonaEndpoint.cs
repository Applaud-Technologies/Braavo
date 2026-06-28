using System.Security.Claims;
using Braavo.Core.UseCases.Personas;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Personas;

public record CreatePersonaRequest(
    string Name,
    string Role,
    string TechnicalLevel = "Medium",
    string[]? Goals = null,
    string[]? PainPoints = null,
    string? Quote = null
);

public class CreatePersonaEndpoint : Endpoint<CreatePersonaRequest, CreatePersonaResult>
{
    private readonly IMediator _mediator;

    public CreatePersonaEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/products/{productId}/personas");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CreatePersonaRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var productId = Route<Guid>("productId");

        var command = new CreatePersonaCommand(
            productId,
            userId,
            req.Name,
            req.Role,
            req.TechnicalLevel,
            req.Goals,
            req.PainPoints,
            req.Quote
        );
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            await SendAsync(result, 400, ct);
            return;
        }

        await SendCreatedAtAsync<ListPersonasEndpoint>(
            new { productId },
            result,
            cancellation: ct
        );
    }
}

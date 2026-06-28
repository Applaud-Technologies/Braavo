using System.Security.Claims;
using Braavo.Core.Entities;
using Braavo.Core.UseCases.Personas;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Personas;

public record UpdatePersonaRequest(
    string Name,
    string Role,
    TechnicalLevel TechnicalLevel,
    string[] Goals,
    string[] PainPoints,
    string Quote
);

public class UpdatePersonaEndpoint : Endpoint<UpdatePersonaRequest, UpdatePersonaResult>
{
    private readonly IMediator _mediator;

    public UpdatePersonaEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/products/{productId}/personas/{id}");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(UpdatePersonaRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var productId = Route<Guid>("productId");
        var personaId = Route<Guid>("id");

        var command = new UpdatePersonaCommand(
            personaId, productId, userId,
            req.Name, req.Role, req.TechnicalLevel,
            req.Goals, req.PainPoints, req.Quote
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

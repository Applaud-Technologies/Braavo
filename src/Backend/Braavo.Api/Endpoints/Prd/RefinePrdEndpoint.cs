using System.Security.Claims;
using Braavo.Core.UseCases.Prd;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Prd;

public record RefineRequest(string Instruction);

public class RefinePrdEndpoint : Endpoint<RefineRequest, RefinePrdResponse>
{
    private readonly IMediator _mediator;

    public RefinePrdEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/api/prd/{id}/refine");
        Policies("Authenticated");
    }

    public override async Task HandleAsync(RefineRequest req, CancellationToken ct)
    {
        var documentId = Route<Guid>("id");
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var command = new RefinePrdCommand(documentId, req.Instruction, userId);
        var result = await _mediator.Send(command, ct);

        if (result.Success)
            await SendOkAsync(result, ct);
        else if (result.Error == "Document not found")
            await SendNotFoundAsync(ct);
        else if (result.Error == "Forbidden")
        {
            await SendForbiddenAsync(ct);
            return;
        }
        else
            await SendAsync(result, 500, ct);
    }
}

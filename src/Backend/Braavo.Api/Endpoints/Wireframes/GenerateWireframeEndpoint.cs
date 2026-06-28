using Braavo.Core.UseCases.Wireframes;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Wireframes;

public record WireframeRequest(Guid DocumentId, string? ScreenName, string Fidelity = "low");

public class GenerateWireframeEndpoint : Endpoint<WireframeRequest, WireframeResponse>
{
    private readonly IMediator _mediator;

    public GenerateWireframeEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/api/wireframes/generate");
        Policies("Authenticated");
    }

    public override async Task HandleAsync(WireframeRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var command = new GenerateWireframeCommand(req.DocumentId, userId, req.ScreenName, req.Fidelity);
        var result = await _mediator.Send(command, ct);

        if (result.Success)
            await SendOkAsync(result, ct);
        else if (result.Error == "Document not found")
            await SendNotFoundAsync(ct);
        else if (result.Error == "Forbidden")
            await SendForbiddenAsync(ct);
        else
            await SendAsync(result, 500, ct);
    }
}

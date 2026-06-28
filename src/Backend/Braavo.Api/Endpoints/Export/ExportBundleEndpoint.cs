using Braavo.Core.UseCases.Export;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Export;

public class ExportRequest
{
    public Guid DocumentId { get; set; }
    public bool IncludeDiagrams { get; set; } = false;
}

public class ExportBundleEndpoint : Endpoint<ExportRequest>
{
    private readonly IMediator _mediator;

    public ExportBundleEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Get("/api/export");
        Policies("Authenticated");
    }

    public override async Task HandleAsync(ExportRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var command = new ExportBundleCommand(req.DocumentId, req.IncludeDiagrams, userId);
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            if (result.Error == "Forbidden")
            {
                await SendForbiddenAsync(ct);
                return;
            }
            if (result.Error == "Document not found")
            {
                await SendNotFoundAsync(ct);
                return;
            }
            await SendAsync(new { error = result.Error }, 500, ct);
            return;
        }

        await SendBytesAsync(
            result.ZipContent,
            result.FileName,
            "application/zip",
            cancellation: ct
        );
    }
}

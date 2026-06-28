using Braavo.Core.UseCases.Export;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Export;

public class ExportPdfRequest
{
    public Guid Id { get; set; }
}

public class ExportPdfEndpoint : Endpoint<ExportPdfRequest>
{
    private readonly IMediator _mediator;

    public ExportPdfEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Get("/api/products/{Id}/export/pdf");
        Policies("Authenticated");
    }

    public override async Task HandleAsync(ExportPdfRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var query = new ExportPdfQuery(req.Id, userId);
        var result = await _mediator.Send(query, ct);

        if (!result.Success)
        {
            if (result.Error == "Forbidden")
            {
                await SendForbiddenAsync(ct);
                return;
            }
            if (result.Error == "Product not found")
            {
                await SendNotFoundAsync(ct);
                return;
            }
            await SendAsync(new { error = result.Error }, 500, ct);
            return;
        }

        await SendBytesAsync(
            result.Content!,
            result.FileName!,
            "application/pdf",
            cancellation: ct
        );
    }
}

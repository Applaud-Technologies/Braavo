using Braavo.Core.UseCases.Export;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Export;

public class ExportMarkdownRequest
{
    public Guid Id { get; set; }
}

public class ExportMarkdownEndpoint : Endpoint<ExportMarkdownRequest>
{
    private readonly IMediator _mediator;

    public ExportMarkdownEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Get("/api/products/{Id}/export/markdown");
        Policies("Authenticated");
    }

    public override async Task HandleAsync(ExportMarkdownRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var query = new ExportMarkdownQuery(req.Id, userId);
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
            "text/markdown",
            cancellation: ct
        );
    }
}

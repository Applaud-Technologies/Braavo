using Braavo.Core.UseCases.Export;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Export;

public class ExportBundleEndpoint : EndpointWithoutRequest
{
    private readonly IMediator _mediator;

    public ExportBundleEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Get("/api/export/{documentId}");
        Policies("Authenticated");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var documentId = Route<Guid>("documentId");
        var command = new ExportBundleCommand(documentId);
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            if (result.Error == "Document not found")
                await SendNotFoundAsync(ct);
            else
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

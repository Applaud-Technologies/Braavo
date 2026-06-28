using Braavo.Core.UseCases.Diagrams;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Diagrams;

public record DiagramRequest(Guid DocumentId, string Type, string? Focus);

public class GenerateDiagramEndpoint : Endpoint<DiagramRequest, DiagramResponse>
{
    private readonly IMediator _mediator;

    public GenerateDiagramEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/api/diagrams/generate");
        Policies("Authenticated");
    }

    public override async Task HandleAsync(DiagramRequest req, CancellationToken ct)
    {
        if (!Enum.TryParse<DiagramType>(req.Type, true, out var diagramType))
        {
            await SendAsync(new DiagramResponse("", DiagramType.Flowchart, false, "Invalid diagram type"), 400, ct);
            return;
        }

        var command = new GenerateDiagramCommand(req.DocumentId, diagramType, req.Focus);
        var result = await _mediator.Send(command, ct);

        if (result.Success)
            await SendOkAsync(result, ct);
        else if (result.Error == "Document not found")
            await SendNotFoundAsync(ct);
        else
            await SendAsync(result, 500, ct);
    }
}

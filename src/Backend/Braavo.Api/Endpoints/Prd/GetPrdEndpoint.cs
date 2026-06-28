using Braavo.Core.Interfaces;
using Braavo.Core.Models;
using Braavo.Core.UseCases.Prd;
using FastEndpoints;

namespace Braavo.Api.Endpoints.Prd;

public class GetPrdEndpoint : EndpointWithoutRequest<PrdResponse>
{
    private readonly IDocumentRepository _documents;

    public GetPrdEndpoint(IDocumentRepository documents) => _documents = documents;

    public override void Configure()
    {
        Get("/api/prd/{id}");
        Policies("Authenticated");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var document = await _documents.GetByIdAsync(id, ct);

        if (document is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var parsed = PrdParser.Parse(document.Content);
        var response = new PrdResponse(
            document.Id,
            document.Title,
            document.Content,
            parsed,
            document.Version,
            document.CreatedAt,
            document.UpdatedAt
        );

        await SendOkAsync(response, ct);
    }
}

public record PrdResponse(
    Guid Id,
    string Title,
    string RawContent,
    PrdContent Parsed,
    int Version,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

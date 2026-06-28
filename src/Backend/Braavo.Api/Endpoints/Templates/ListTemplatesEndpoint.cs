using Braavo.Core.Interfaces;
using FastEndpoints;

namespace Braavo.Api.Endpoints.Templates;

public record TemplateDto(Guid Id, string Name, string Description, string Category, string PromptHint);

public class ListTemplatesEndpoint : EndpointWithoutRequest<List<TemplateDto>>
{
    private readonly ITemplateRepository _templates;

    public ListTemplatesEndpoint(ITemplateRepository templates) => _templates = templates;

    public override void Configure()
    {
        Get("/api/templates");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var templates = await _templates.GetAllAsync(ct);
        var dtos = templates.Select(t => new TemplateDto(
            t.Id, t.Name, t.Description, t.Category, t.PromptHint
        )).ToList();

        await SendOkAsync(dtos, ct);
    }
}

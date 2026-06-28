using Braavo.Core.Interfaces;
using FastEndpoints;

namespace Braavo.Api.Endpoints.Templates;

public record TemplateDetailDto(
    Guid Id, string Name, string Description, string Category,
    string Content, string PromptHint, bool IsSystem);

public class GetTemplateEndpoint : EndpointWithoutRequest<TemplateDetailDto>
{
    private readonly ITemplateRepository _templates;

    public GetTemplateEndpoint(ITemplateRepository templates) => _templates = templates;

    public override void Configure()
    {
        Get("/api/templates/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var template = await _templates.GetByIdAsync(id, ct);

        if (template is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var dto = new TemplateDetailDto(
            template.Id, template.Name, template.Description,
            template.Category, template.Content, template.PromptHint, template.IsSystem
        );

        await SendOkAsync(dto, ct);
    }
}

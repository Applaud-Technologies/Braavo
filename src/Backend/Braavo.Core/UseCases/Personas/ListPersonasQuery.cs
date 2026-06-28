using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using Braavo.Core.UseCases.Products;
using MediatR;

namespace Braavo.Core.UseCases.Personas;

public record ListPersonasQuery(Guid ProductId, Guid UserId) : IRequest<ListPersonasResult>;

public record ListPersonasResult(
    bool Success,
    IReadOnlyList<PersonaDto>? Personas = null,
    string? Error = null
);

public class ListPersonasHandler : IRequestHandler<ListPersonasQuery, ListPersonasResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IPersonaRepository _personaRepo;

    public ListPersonasHandler(IProductRepository productRepo, IPersonaRepository personaRepo)
    {
        _productRepo = productRepo;
        _personaRepo = personaRepo;
    }

    public async Task<ListPersonasResult> Handle(ListPersonasQuery request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new ListPersonasResult(false, Error: "Product not found");

        var personas = await _personaRepo.GetByProductIdAsync(request.ProductId, ct);

        var dtos = personas.Select(p => new PersonaDto(
            p.Id, p.Name, p.Role, p.TechnicalLevel.ToString(),
            p.Goals, p.PainPoints, p.Quote
        )).ToList();

        return new ListPersonasResult(true, dtos);
    }
}

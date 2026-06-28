using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Personas;

public record CreatePersonaCommand(
    Guid ProductId,
    Guid UserId,
    string Name,
    string Role,
    string TechnicalLevel = "Medium",
    string[]? Goals = null,
    string[]? PainPoints = null,
    string? Quote = null
) : IRequest<CreatePersonaResult>;

public record CreatePersonaResult(
    Guid PersonaId,
    bool Success,
    string? Error = null
);

public class CreatePersonaHandler : IRequestHandler<CreatePersonaCommand, CreatePersonaResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IPersonaRepository _personaRepo;

    public CreatePersonaHandler(IProductRepository productRepo, IPersonaRepository personaRepo)
    {
        _productRepo = productRepo;
        _personaRepo = personaRepo;
    }

    public async Task<CreatePersonaResult> Handle(CreatePersonaCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new CreatePersonaResult(Guid.Empty, false, "Product not found");

        if (string.IsNullOrWhiteSpace(request.Name))
            return new CreatePersonaResult(Guid.Empty, false, "Persona name is required");

        if (string.IsNullOrWhiteSpace(request.Role))
            return new CreatePersonaResult(Guid.Empty, false, "Persona role is required");

        var techLevel = Enum.TryParse<TechnicalLevel>(request.TechnicalLevel, true, out var level)
            ? level
            : TechnicalLevel.Medium;

        var persona = Persona.Create(
            request.ProductId,
            request.Name,
            request.Role,
            techLevel,
            request.Goals,
            request.PainPoints,
            request.Quote);
        await _personaRepo.AddAsync(persona, ct);

        return new CreatePersonaResult(persona.Id, true);
    }
}

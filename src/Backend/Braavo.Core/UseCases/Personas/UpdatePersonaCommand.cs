using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Personas;

public record UpdatePersonaCommand(
    Guid PersonaId,
    Guid ProductId,
    Guid UserId,
    string Name,
    string Role,
    TechnicalLevel TechnicalLevel,
    string[] Goals,
    string[] PainPoints,
    string Quote
) : IRequest<UpdatePersonaResult>;

public record UpdatePersonaResult(bool Success, string? Error = null);

public class UpdatePersonaHandler : IRequestHandler<UpdatePersonaCommand, UpdatePersonaResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IPersonaRepository _personaRepo;

    public UpdatePersonaHandler(IProductRepository productRepo, IPersonaRepository personaRepo)
    {
        _productRepo = productRepo;
        _personaRepo = personaRepo;
    }

    public async Task<UpdatePersonaResult> Handle(UpdatePersonaCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new UpdatePersonaResult(false, "Product not found");

        var persona = await _personaRepo.GetByIdAsync(request.PersonaId, ct);
        if (persona is null || persona.ProductId != request.ProductId)
            return new UpdatePersonaResult(false, "Persona not found");

        if (string.IsNullOrWhiteSpace(request.Name))
            return new UpdatePersonaResult(false, "Persona name is required");

        persona.Update(request.Name, request.Role, request.TechnicalLevel, request.Goals, request.PainPoints, request.Quote);
        await _personaRepo.UpdateAsync(persona, ct);

        return new UpdatePersonaResult(true);
    }
}

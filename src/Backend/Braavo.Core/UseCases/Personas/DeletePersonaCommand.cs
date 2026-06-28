using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Personas;

public record DeletePersonaCommand(
    Guid PersonaId,
    Guid ProductId,
    Guid UserId
) : IRequest<DeletePersonaResult>;

public record DeletePersonaResult(bool Success, string? Error = null);

public class DeletePersonaHandler : IRequestHandler<DeletePersonaCommand, DeletePersonaResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IPersonaRepository _personaRepo;

    public DeletePersonaHandler(IProductRepository productRepo, IPersonaRepository personaRepo)
    {
        _productRepo = productRepo;
        _personaRepo = personaRepo;
    }

    public async Task<DeletePersonaResult> Handle(DeletePersonaCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new DeletePersonaResult(false, "Product not found");

        var persona = await _personaRepo.GetByIdAsync(request.PersonaId, ct);
        if (persona is null || persona.ProductId != request.ProductId)
            return new DeletePersonaResult(false, "Persona not found");

        await _personaRepo.DeleteAsync(request.PersonaId, ct);

        return new DeletePersonaResult(true);
    }
}

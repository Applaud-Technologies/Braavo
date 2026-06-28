using Braavo.Core.Entities;

namespace Braavo.Core.Interfaces;

public interface IPersonaRepository
{
    Task<Persona?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Persona>> GetByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task AddAsync(Persona persona, CancellationToken ct = default);
    Task UpdateAsync(Persona persona, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

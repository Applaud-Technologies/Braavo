using Braavo.Core.Entities;

namespace Braavo.Core.Interfaces;

public interface IFeatureRepository
{
    Task<Feature?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Feature>> GetByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task<IReadOnlyList<Feature>> GetByPhaseAsync(Guid productId, FeaturePhase phase, CancellationToken ct = default);
    Task AddAsync(Feature feature, CancellationToken ct = default);
    Task UpdateAsync(Feature feature, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

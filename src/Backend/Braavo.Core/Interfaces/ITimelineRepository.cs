using Braavo.Core.Entities;

namespace Braavo.Core.Interfaces;

public interface ITimelineRepository
{
    Task<IReadOnlyList<TimelinePhase>> GetByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task<TimelinePhase?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(TimelinePhase phase, CancellationToken ct = default);
    Task UpdateAsync(TimelinePhase phase, CancellationToken ct = default);
    Task DeleteByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task ReplaceTimelineAsync(Guid productId, IEnumerable<TimelinePhase> phases, CancellationToken ct = default);
}

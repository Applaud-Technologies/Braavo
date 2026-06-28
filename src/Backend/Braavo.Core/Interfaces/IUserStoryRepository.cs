using Braavo.Core.Entities;

namespace Braavo.Core.Interfaces;

public interface IUserStoryRepository
{
    Task<UserStory?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<UserStory>> GetByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task<IReadOnlyList<UserStory>> GetByPersonaIdAsync(Guid personaId, CancellationToken ct = default);
    Task AddAsync(UserStory story, CancellationToken ct = default);
    Task UpdateAsync(UserStory story, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

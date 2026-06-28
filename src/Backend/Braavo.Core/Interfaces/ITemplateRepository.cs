using Braavo.Core.Entities;

namespace Braavo.Core.Interfaces;

public interface ITemplateRepository
{
    Task<IReadOnlyList<Template>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Template>> GetByCategoryAsync(string category, CancellationToken ct = default);
    Task<Template?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Template template, CancellationToken ct = default);
}

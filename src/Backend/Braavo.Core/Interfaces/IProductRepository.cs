using Braavo.Core.Entities;
using Braavo.Core.ValueObjects;

namespace Braavo.Core.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetByOwnerIdAsync(UserId ownerId, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
    Task UpdateAsync(Product product, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task AddVersionAsync(ProductVersion version, CancellationToken ct = default);
    Task<IReadOnlyList<ProductVersion>> GetVersionsAsync(Guid productId, CancellationToken ct = default);
}

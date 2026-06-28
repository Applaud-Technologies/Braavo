using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using Braavo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Braavo.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly BraavoDbContext _context;

    public ProductRepository(BraavoDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IReadOnlyList<Product>> GetByOwnerIdAsync(UserId ownerId, CancellationToken ct = default)
    {
        return await _context.Products
            .Where(p => p.OwnerId == ownerId)
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Product product, CancellationToken ct = default)
    {
        await _context.Products.AddAsync(product, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await GetByIdAsync(id, ct);
        if (product is not null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task AddVersionAsync(ProductVersion version, CancellationToken ct = default)
    {
        await _context.ProductVersions.AddAsync(version, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ProductVersion>> GetVersionsAsync(Guid productId, CancellationToken ct = default)
    {
        return await _context.ProductVersions
            .Where(v => v.ProductId == productId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync(ct);
    }
}

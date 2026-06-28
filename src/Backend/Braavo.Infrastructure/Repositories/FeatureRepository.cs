using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Braavo.Infrastructure.Repositories;

public class FeatureRepository : IFeatureRepository
{
    private readonly BraavoDbContext _context;

    public FeatureRepository(BraavoDbContext context)
    {
        _context = context;
    }

    public async Task<Feature?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Features.FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<IReadOnlyList<Feature>> GetByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        return await _context.Features
            .Where(f => f.ProductId == productId)
            .OrderBy(f => f.Phase)
            .ThenBy(f => f.SortOrder)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Feature>> GetByPhaseAsync(Guid productId, FeaturePhase phase, CancellationToken ct = default)
    {
        return await _context.Features
            .Where(f => f.ProductId == productId && f.Phase == phase)
            .OrderBy(f => f.SortOrder)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Feature feature, CancellationToken ct = default)
    {
        await _context.Features.AddAsync(feature, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Feature feature, CancellationToken ct = default)
    {
        _context.Features.Update(feature);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var feature = await GetByIdAsync(id, ct);
        if (feature is not null)
        {
            _context.Features.Remove(feature);
            await _context.SaveChangesAsync(ct);
        }
    }
}

using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Braavo.Infrastructure.Repositories;

public class TimelineRepository : ITimelineRepository
{
    private readonly BraavoDbContext _context;

    public TimelineRepository(BraavoDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TimelinePhase>> GetByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        return await _context.TimelinePhases
            .Where(p => p.ProductId == productId)
            .Include(p => p.Milestones)
            .OrderBy(p => p.SortOrder)
            .ToListAsync(ct);
    }

    public async Task<TimelinePhase?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.TimelinePhases
            .Include(p => p.Milestones)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task AddAsync(TimelinePhase phase, CancellationToken ct = default)
    {
        await _context.TimelinePhases.AddAsync(phase, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TimelinePhase phase, CancellationToken ct = default)
    {
        _context.TimelinePhases.Update(phase);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        var phases = await _context.TimelinePhases
            .Where(p => p.ProductId == productId)
            .ToListAsync(ct);
        _context.TimelinePhases.RemoveRange(phases);
        await _context.SaveChangesAsync(ct);
    }

    public async Task ReplaceTimelineAsync(Guid productId, IEnumerable<TimelinePhase> phases, CancellationToken ct = default)
    {
        var existing = await _context.TimelinePhases
            .Where(p => p.ProductId == productId)
            .ToListAsync(ct);
        _context.TimelinePhases.RemoveRange(existing);
        await _context.TimelinePhases.AddRangeAsync(phases, ct);
        await _context.SaveChangesAsync(ct);
    }
}

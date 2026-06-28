using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using Braavo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Braavo.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly BraavoDbContext _context;

    public DocumentRepository(BraavoDbContext context) => _context = context;

    public async Task<Document?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Documents.FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<IReadOnlyList<Document>> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default)
        => await _context.Documents.Where(d => d.ProjectId == projectId).ToListAsync(ct);

    public async Task<IReadOnlyList<Document>> GetByUserIdAsync(UserId userId, int skip, int take, CancellationToken ct = default)
        => await _context.Documents
            .Where(d => d.CreatedBy == userId)
            .OrderByDescending(d => d.UpdatedAt)
            .Skip(skip).Take(take)
            .ToListAsync(ct);

    public async Task AddAsync(Document document, CancellationToken ct = default)
    {
        await _context.Documents.AddAsync(document, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Document document, CancellationToken ct = default)
    {
        _context.Documents.Update(document);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var doc = await GetByIdAsync(id, ct);
        if (doc is not null)
        {
            _context.Documents.Remove(doc);
            await _context.SaveChangesAsync(ct);
        }
    }
}

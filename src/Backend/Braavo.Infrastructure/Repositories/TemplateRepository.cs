using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Braavo.Infrastructure.Repositories;

public class TemplateRepository : ITemplateRepository
{
    private readonly BraavoDbContext _context;

    public TemplateRepository(BraavoDbContext context) => _context = context;

    public async Task<IReadOnlyList<Template>> GetAllAsync(CancellationToken ct = default)
        => await _context.Templates.OrderBy(t => t.Category).ThenBy(t => t.Name).ToListAsync(ct);

    public async Task<IReadOnlyList<Template>> GetByCategoryAsync(string category, CancellationToken ct = default)
        => await _context.Templates.Where(t => t.Category == category).ToListAsync(ct);

    public async Task<Template?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Templates.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task AddAsync(Template template, CancellationToken ct = default)
    {
        await _context.Templates.AddAsync(template, ct);
        await _context.SaveChangesAsync(ct);
    }
}

using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Braavo.Infrastructure.Repositories;

public class PersonaRepository : IPersonaRepository
{
    private readonly BraavoDbContext _context;

    public PersonaRepository(BraavoDbContext context)
    {
        _context = context;
    }

    public async Task<Persona?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Personas.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IReadOnlyList<Persona>> GetByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        return await _context.Personas
            .Where(p => p.ProductId == productId)
            .OrderBy(p => p.SortOrder)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Persona persona, CancellationToken ct = default)
    {
        await _context.Personas.AddAsync(persona, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Persona persona, CancellationToken ct = default)
    {
        _context.Personas.Update(persona);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var persona = await GetByIdAsync(id, ct);
        if (persona is not null)
        {
            _context.Personas.Remove(persona);
            await _context.SaveChangesAsync(ct);
        }
    }
}
